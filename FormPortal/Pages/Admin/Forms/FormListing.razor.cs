using Blazor.Pagination;
using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using FormPortal.Core.Models;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;

namespace FormPortal.Pages.Admin.Forms
{
    public partial class FormListing : IHasPagination
    {
        public FormFilter Filter { get; set; } = new FormFilter();

        public List<Form> Data { get; set; } = new();
        [Parameter]
        public int Page { get; set; }
        public int TotalItems { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
        }
        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo("/Admin/Forms/");
            }

            Filter.PageNumber = Page;
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            TotalItems = await formService.GetTotalAsync(Filter, dbController);
            Data = await formService.GetAsync(Filter, dbController);

        }

    }
}