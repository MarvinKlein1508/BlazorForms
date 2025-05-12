using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace BlazorForms.Web.Components.Pages.Account;

public partial class Login
{
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    [SupplyParameterFromForm]
    public LoginInput Input { get; set; } = new LoginInput();

    [SupplyParameterFromQuery]
    private string? ReturnUrl { get; set; }

    private string? _errorMessage;

    public async Task HandleLogin()
    {
    }
}

public class LoginInput
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}