using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace FormPortal.Core.Services
{
    public static class AppdatenService
    {
        public static string[] SupportedCultureCodes => SupportedCultures.Select(x => x.Name).ToArray();

        public static CultureInfo[] SupportedCultures => new CultureInfo[]
        {
            new CultureInfo("de-de"),
            new CultureInfo("en-US")
        };


        public static bool FirstUserExists { get; set; } = false;
        public static List<FormElement> Elements { get; } = new List<FormElement>
        {
            new FormTextElement { Name = "Text" },
            new FormDateElement { Name = "Date" },
            new FormSelectElement { Name = "Select" },
            new FormCheckboxElement { Name = "Checkbox"},
            new FormTextareaElement { Name = "Textarea" },
            new FormFileElement { Name = "File"},
            new FormRadioElement { Name = "Radio"},
            new FormNumberElement { Name ="Number"},
            new FormTableElement { Name = "Table"},
            new FormLabelElement { Name = "Label"}
        };

        public static List<Permission> Permissions { get; set; } = new();

        private static IConfiguration? _configuration;

        public static async Task InitAsync(IConfiguration configuration, IDbProviderService dbProviderService)
        {
            _configuration = configuration;
            using IDbController dbController = dbProviderService.GetDbController(ConnectionString);
            Permissions = await PermissionService.GetAllAsync(dbController);
            FirstUserExists = await UserService.FirstUserExistsAsync(dbController);
        }
        public static string ConnectionString => _configuration?["ConnectionString"] ?? string.Empty;
        public static bool IsLdapLoginEnabled => _configuration?.GetSection("LdapSettings").GetValue<bool>("ENABLE_LDAP_LOGIN") ?? false;
        public static bool IsLocalLoginEnabled => _configuration?.GetSection("LdapSettings").GetValue<bool>("ENABLE_LOCAL_LOGIN") ?? false;
        public static string LdapServer => _configuration?["LdapSettings:LDAP_SERVER"] ?? string.Empty;
        public static string LdapDomainServer => _configuration?["LdapSettings:DOMAIN_SERVER"] ?? string.Empty;
        public static string LdapDistinguishedName => _configuration?["LdapSettings:DistinguishedName"] ?? string.Empty;

        public static Dictionary<string, string> MimeTypes => _configuration?.GetSection("MimeTypes").GetChildren().ToDictionary(x => x.Key, x => x.Value!) ?? new Dictionary<string, string>();
        public static int PageLimit => _configuration?.GetValue<int>("PageLimit") ?? 30;


        public static CultureInfo ToCulture(this ILocalizationHelper helper)
        {

            var culture = SupportedCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName.Equals(helper.Code, StringComparison.OrdinalIgnoreCase));

            if (culture is null)
            {
                return SupportedCultures[0];
            }
            else
            {
                return culture;
            }
        }
    }
}
