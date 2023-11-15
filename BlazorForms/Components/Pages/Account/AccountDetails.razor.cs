using BlazorForms.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorForms.Components.Pages.Account
{
    public partial class AccountDetails
    {
        [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;
        public User? CurrentUser { get; set; }
        private ClaimsPrincipal? _user;
        protected override async Task OnParametersSetAsync()
        {
            if (AuthState is not null)
            {
                _user = (await AuthState).User;

                CurrentUser = await authService.GetUserAsync();
            }
        }
    }
}