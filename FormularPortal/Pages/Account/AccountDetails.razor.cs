using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using FormularPortal;
using FormularPortal.Components;
using Plk.Blazor.DragDrop;
using FormularPortal.Core.Services;
using FormularPortal.Core.Models;
using FormularPortal.Components.Modals;
using DatabaseControllerProvider;
using Blazor.Pagination;
using System.Security.Claims;

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