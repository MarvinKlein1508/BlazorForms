using System.ComponentModel.DataAnnotations;
using BlazorForms.Domain;
using BlazorForms.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using OtpNet;
using QRCoder;

namespace BlazorForms.Components.Pages.Account;

public partial class AccountDetails
{
    private byte[]? _secretKey;
    private string? _base32Secret;
    private string? _totpUri;
    private string? _qrCodeAsBase64;
    private string? _verificationToken;
    private PasswordChangeInput _passwordChangeInput = new();
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (CurrentUser is not null && !CurrentUser.TwoFactorEnabled)
        {
            _secretKey = KeyGeneration.GenerateRandomKey(20);
            _base32Secret = Base32Encoding.ToString(_secretKey);
            _totpUri = $"otpauth://totp/HSOnline:{CurrentUser.Username}?secret={_base32Secret}&issuer=HSOnline&digits=6";
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(_totpUri, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new(qrCodeData);
            _qrCodeAsBase64 = qrCode.GetGraphic(20);
        }
    }
    private async Task DisableTwoFactorAuthenticationAsync()
    {
        if (CurrentUser is null)
        {
            return;
        }

        var result = await DialogService.ShowConfirmationAsync("Sind Sie sicher, dass Sie die Zwei-Faktor-Authentifizierung deaktivieren möchten?", "Zwei-Faktor-Authentifizierung deaktivieren");

        if (!result.Cancelled)
        {
            using var connection = await DbFactory.CreateConnectionAsync();
            CurrentUser.TwoFactorEnabled = false;
            CurrentUser.TwoFactorToken = null;
            await userRepository.SetTwoFactorAuthenticationAsync(CurrentUser, connection);
            await DialogService.ShowSuccessAsync("Zwei-Faktor-Authentifizierung wurde erfolgreich deaktiviert", "Zwei-Faktor-Authentifizierung deaktiviert");
        }
    }

    private async Task EnableTwoFactorAuthenticationAsync()
    {
        if (CurrentUser is null)
        {
            return;
        }

        var totp = new Totp(_secretKey);

        if (!totp.VerifyTotp(_verificationToken, out long _, VerificationWindow.RfcSpecifiedNetworkDelay))
        {
            await DialogService.ShowErrorAsync("Sie haben einen falschen oder abgelaufenen Code eingegeben. Bitte versuchen Sie es erneut", "Ungültiger Code");
            return;
        }

        CurrentUser.TwoFactorToken = _base32Secret;
        CurrentUser.TwoFactorEnabled = true;
        using var connection = await DbFactory.CreateConnectionAsync();
        await userRepository.SetTwoFactorAuthenticationAsync(CurrentUser, connection);
        await DialogService.ShowSuccessAsync("Sie haben die Zwei-Faktor-Authentifizierung erfolgreich aktiviert!", "Zwei-Faktor-Authentifizierung aktiviert");
    }

    private async Task ChangePasswordAsync()
    {
        if (CurrentUser is null)
        {
            return;
        }

        var user = await loginService.LoginAsync(CurrentUser.Username, _passwordChangeInput.CurrentPassword);

        if (user is null)
        {
            await DialogService.ShowErrorAsync("Das eingegebene Passwort stimmt nicht mit Ihrem aktuellen Passwort überein!", "Falsches Passwort");
            return;
        }

        user.Salt = SaltGenerator.GenerateSaltBase64();
        PasswordHasher<User> hasher = new();
        string passwordHashed = hasher.HashPassword(user, _passwordChangeInput.NewPassword + user.Salt);
        user.Password = passwordHashed;

        using var connection = await DbFactory.CreateConnectionAsync();
        await userRepository.ChangePasswordAsync(user, connection);
        await DialogService.ShowSuccessAsync("Ihr Passwort wurde erfolgreich geändert!", "Passwort geändert");

    }

    private sealed class PasswordChangeInput
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "Passwörter stimmen nicht überein.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;

    }
}
