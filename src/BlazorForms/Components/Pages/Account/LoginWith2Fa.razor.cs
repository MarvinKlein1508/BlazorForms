using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Claims;
using BlazorForms.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using OtpNet;

namespace BlazorForms.Components.Pages.Account;

public partial class LoginWith2Fa
{

    private string _errorMessage = string.Empty;

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    [SupplyParameterFromQuery]
    private bool RememberMe { get; set; }

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    private InputModel Input { get; set; }

    public LoginWith2Fa()
    {
        Input = new InputModel();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        AuthenticateResult result = await HttpContext.AuthenticateAsync(AuthConstants.TwoFactorCookieName);

        if (result.Succeeded && result.Principal.FindFirst("userId") is Claim claim)
        {
            var userId = Convert.ToInt32(claim.Value, CultureInfo.InvariantCulture);

            using var connection = await DbFactory.CreateConnectionAsync();
            CurrentUser = await userRepository.GetAsync(userId, connection);
        }
    }

    private async Task HandleVerify()
    {
        if (CurrentUser is null)
        {
            return;
        }

        byte[] secretBytes = Base32Encoding.ToBytes(CurrentUser.TwoFactorToken);
        var totp = new Totp(secretBytes);
        if (totp.VerifyTotp(Input.TwoFactorCode, out long _, VerificationWindow.RfcSpecifiedNetworkDelay))
        {
            await HttpContext.LoginAsync(CurrentUser, RememberMe);
            NavigationManager.NavigateTo(ReturnUrl ?? "/");
        }
        else
        {
            _errorMessage = "Ungültiger Code. Bitte versuchen Sie es erneut!";
        }
    }

    private sealed class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string? TwoFactorCode { get; set; }
    }
}
