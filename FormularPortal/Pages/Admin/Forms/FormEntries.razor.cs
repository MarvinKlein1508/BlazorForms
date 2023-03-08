using FormPortal.Core.Services;
using DatabaseControllerProvider;
using Blazor.Pagination;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;
using System.Data;
using FormPortal.Core.Pdf;

namespace FormularPortal.Pages.Admin.Forms
{
    public partial class FormEntries : IHasPagination
    {
        public FormEntryFilter Filter { get; set; } = new();
        public int Page { get; set; } = 1;
        public int TotalItems { get; set; }
        public List<EntryListItem> Data { get; set; } = new();

        public List<EntryListItem> DownloadingList { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            await LoadAsync();
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            Filter.PageNumber = navigateToPage1 ? 1 : Page;
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            TotalItems = await formEntryService.GetTotalAsync(Filter, dbController);
            Data = await formEntryService.GetAsync(Filter, dbController);
        }

        private async Task DownloadAsync(EntryListItem item)
        {
            DownloadingList.Add(item);
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            var entry = await formEntryService.GetAsync(item.EntryId, dbController);
            if (entry is not null)
            {
                var report = await ReportFormEntry.CreateAsync(entry);

                var data = report.GetBytes();

                await downloadService.DownloadFile($"{item.FormName}_{item.EntryId}.pdf", data, "application/pdf");
            }
            DownloadingList.Remove(item);
        }
    }
}