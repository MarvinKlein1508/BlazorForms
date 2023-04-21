using FormPortal.Core.Services;
using DatabaseControllerProvider;
using Blazor.Pagination;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;

namespace FormPortal.Pages.Admin
{
    public partial class FormStatusManagement : IHasPagination
    {
        public FormStatusFilter Filter { get; set; } = new();
        public int Page { get; set; }
        public int TotalItems { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            await LoadAsync();
        }

        protected override async Task SaveAsync()
        {
            await base.SaveAsync();
            await LoadAsync();
        }

        protected override async Task DeleteAsync()
        {
            await base.DeleteAsync();
            await LoadAsync();
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            Filter.PageNumber = navigateToPage1 ? 1 : Page;
            using IDbController dbController = DbProviderService.GetDbController(AppdatenService.ConnectionString);
            TotalItems = await Service.GetTotalAsync(Filter, dbController);
            Data = await Service.GetAsync(Filter, dbController);
        }

        protected override Task NewAsync()
        {
            var newStatus = new FormStatus();

            foreach (var culture in AppdatenService.SupportedCultures)
            {
                newStatus.Description.Add(new FormStatusDescription
                {
                    Code = culture.TwoLetterISOLanguageName
                });
            }

            Input = newStatus;

            return Task.CompletedTask;
        }

    }
}