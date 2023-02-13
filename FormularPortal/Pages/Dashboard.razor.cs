using Microsoft.AspNetCore.Components;
using FormularPortal.Core.Services;
using FormularPortal.Core.Models;
using DatabaseControllerProvider;
using Blazor.Pagination;
using FormularPortal.Core.Filters;

namespace FormularPortal.Pages
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
        protected override async Task OnParametersSetAsync()
        {
            if(Page < 1)
            {
                Page = 1;
            }

            await LoadAsync();
        }
        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo("/");
            }

            Filter.PageNumber = Page;
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            TotalItems = await formService.GetTotalAsync(Filter, dbController);
            Data = await formService.GetAsync(Filter, dbController);
        }
    }
}