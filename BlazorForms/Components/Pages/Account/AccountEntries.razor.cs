using DbController;
using DbController.MySql;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using BlazorForms.Core;

namespace BlazorForms.Components.Pages.Account
{
    public partial class AccountEntries
    {
        public FormEntryFilter Filter { get; set; } = new()
        {
            Limit = Storage.PageLimit
        };

        [Parameter]
        public int Page { get; set; } = 1;
        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            Filter.PageNumber = Page;

            using IDbController dbController = new MySqlController();
            var user = await authService.GetUserAsync(dbController);
            Filter.UserId = user?.UserId ?? 0;
        }
    }
}