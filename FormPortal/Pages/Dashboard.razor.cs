using Blazor.Pagination;
using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using FormPortal.Core.Models;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;

namespace FormPortal.Pages
{
    public partial class Dashboard : IHasPagination
    {
        public FormFilter Filter { get; set; } = new()
        {
            ShowOnlyActiveForms = true
        };
        [Parameter]
        public int Page { get; set; } = 1;
        public int TotalItems { get; set; }

        public List<Form> Data { get; set; } = new();
        private User? _user;
        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            _user = await authService.GetUserAsync(dbController);

            await LoadAsync();
        }
        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo("/");
            }

            Filter.HideLoginRequired = _user is null;

            Filter.PageNumber = Page;
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            TotalItems = await formService.GetTotalAsync(Filter, dbController);
            Data = await formService.GetAsync(Filter, dbController);
        }
    }
}