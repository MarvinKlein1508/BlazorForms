using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using FormPortal.Core.Models;

namespace FormularPortal.Pages.Account
{
    public partial class AccountDetails
    {
#nullable disable
        [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }
#nullable enable

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