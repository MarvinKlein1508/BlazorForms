using FormPortal.Core.Services;
using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using Microsoft.AspNetCore.Components;

namespace FormularPortal.Pages.Account
{
    public partial class AccountEntries
    {
        public FormEntryFilter Filter { get; set; } = new();

        [Parameter]
        public int Page { get; set; } = 1;
        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            Filter.PageNumber = Page;

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            var user = await authService.GetUserAsync(dbController);
            Filter.UserId = user?.UserId ?? 0;
        }
    }
}