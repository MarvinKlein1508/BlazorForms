using FormularPortal.Core.Services;
using FormularPortal.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Xml.Linq;
using System.DirectoryServices.Protocols;
using FormularPortal.Core.Models;
using DatabaseControllerProvider;

namespace FormularPortal.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly DbProviderService _dbProviderService;
        private readonly UserService _userService;

        [BindProperty]
        public LoginInput Input { get; set; } = new LoginInput();
        public string? ReturnUrl { get; set; }

        public LoginModel(DbProviderService dbProviderService, UserService userService)
        {
            _dbProviderService = dbProviderService;
            _userService = userService;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = GetReturnUrl(returnUrl);

            try
            {
                // Clear the existing external cookie
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch { }

        }


        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = GetReturnUrl(returnUrl);

            if (ModelState.IsValid)
            {

                // Erst prüfen wir gegen die Datenbank
                IDbController dbController = _dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
                User? mitarbeiter = AppdatenService.IsLocalLoginEnabled ? await _userService.GetAsync(Input.Username, dbController) : null;

                // Lokale Konten müssen als ersten geprüft werden.
                if (mitarbeiter is not null)
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();

                    string passwordHashed = hasher.HashPassword(mitarbeiter, Input.Password + mitarbeiter.Salt);

                    PasswordVerificationResult result = hasher.VerifyHashedPassword(mitarbeiter, mitarbeiter.Password, Input.Password + mitarbeiter.Salt);
                    // Das Handling läuft später auf Basis des Objektes ab.
                    if (result is PasswordVerificationResult.Failed)
                    {
                        mitarbeiter = null;
                    }
                }
                else
                {
                    // Wenn kein lokales Konto gefunden wurde, dann prüfen wir das Active-Directory
                    if (AppdatenService.IsLdapLoginEnabled)
                    {
                        try
                        {
                            using var connection = new LdapConnection(AppdatenService.LdapServer);

                            var networkCredential = new NetworkCredential(Input.Username, Input.Password, AppdatenService.LdapDomainServer);
                            connection.SessionOptions.SecureSocketLayer = false; // Warnung kann ignoriert werden, ist ein Fehler vom Package
                            connection.AuthType = AuthType.Negotiate;
                            connection.Bind(networkCredential);

                            var searchRequest = new SearchRequest
                                (
                                AppdatenService.LdapDistinguishedName,
                $"(SAMAccountName={Input.Username})",
                                SearchScope.Subtree, new string[]
                                {
                                "cn",
                                "mail",
                                "givenName",
                                "sn",
                                "objectGUID"
                                });

                            SearchResponse directoryResponse = (SearchResponse)connection.SendRequest(searchRequest);

                            SearchResultEntry searchResultEntry = directoryResponse.Entries[0];

                            Dictionary<string, string> attributes = new Dictionary<string, string>();
                            Guid? guid = null;
                            foreach (DirectoryAttribute userReturnAttribute in searchResultEntry.Attributes.Values)
                            {
                                if (userReturnAttribute.Name == "objectGUID")
                                {
                                    byte[] guidByteArray = (byte[])userReturnAttribute.GetValues(typeof(byte[]))[0];
                                    guid = new Guid(guidByteArray);
                                    attributes.Add("guid", ((Guid)guid).ToString());
                                }
                                else
                                {
                                    attributes.Add(userReturnAttribute.Name, (string)userReturnAttribute.GetValues(typeof(string))[0]);
                                }
                            }

                            if (!attributes.ContainsKey("mail"))
                            {
                                attributes.Add("mail", string.Empty);
                            }

                            if (!attributes.ContainsKey("sn"))
                            {
                                attributes.Add("sn", string.Empty);
                            }

                            if (!attributes.ContainsKey("givenName"))
                            {
                                attributes.Add("givenName", string.Empty);
                            }

                            if (guid is null)
                            {
                                throw new InvalidOperationException();
                            }

                            mitarbeiter = await _userService.GetAsync((Guid)guid, dbController);

                            if (mitarbeiter is null)
                            {
                                mitarbeiter = new User
                                {
                                    Username = Input.Username.ToUpper(),
                                    ActiveDirectoryGuid = (Guid)guid,
                                    Email = attributes["mail"],
                                    DisplayName = $"{attributes["givenName"]} {attributes["sn"]}",
                                    Origin = "ad"
                                };

                                // Wenn der erste User angelegt wird, dann sollen diesem alle Berechtigungen gegeben werden
                                //if (!AppdatenService.ActiveDirectoryUserExists)
                                //{
                                //    foreach (var berechtigung in AppdatenService.Berechtigungen)
                                //    {
                                //        mitarbeiter.Berechtigungen.Add(berechtigung);
                                //    }
                                //}

                                await _userService.CreateAsync(mitarbeiter, dbController);
                                //AppdatenService.ActiveDirectoryUserExists = true;
                            }



                        }
                        catch (LdapException ex)
                        {

                        }
                    }
                }

                // Wenn wir ein Mitarbeiter Objekt haben, dann können wir uns einloggen, ansonsten ist irgendwas schief gelaufen
                if (mitarbeiter is not null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("userId", mitarbeiter.UserId.ToString()),
                    };

                    //foreach (var berechtigung in mitarbeiter.Berechtigungen)
                    //{
                    //    claims.Add(new Claim(ClaimTypes.Role, berechtigung.Identifier));
                    //}

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Input.RememberMe,
                        RedirectUri = returnUrl
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    if (!AppdatenService.IsLdapLoginEnabled && !AppdatenService.IsLocalLoginEnabled)
                    {
                        ModelState.AddModelError("login-error", "Es wurde kein Provider zum einloggen gefunden. Bitte wenden Sie sich an Ihren Administrator.");
                    }
                    else
                    {
                        ModelState.AddModelError("login-error", "Username oder Passwort ist falsch.");
                    }
                }
            }
            return Page();
        }

        private string GetReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return Url.Content("~/");
            }
            else
            {
                string[] parts = returnUrl.Split('/');

                string url = string.Empty;

                foreach (var item in parts)
                {
                    url += $"/{HttpUtility.UrlEncode(item)}";
                }

                return url.Replace("//", "/");
            }
        }
    }

    public class LoginInput
    {
        [Required]
        public string Username { get; set; } = String.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = String.Empty;
        [Display(Name = "Eingeloggt bleiben")]
        public bool RememberMe { get; set; }
    }
}
