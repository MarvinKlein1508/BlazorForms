using BlazorForms.Core;
using BlazorForms.Core.Filters;
using DbController;
using DbController.MySql;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components.Pages.Account
{
    public partial class AssignedToMe
    {
        public FormEntryFilter Filter { get; set; } = new FormEntryFilter
        {
            SearchAssigned = true,
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