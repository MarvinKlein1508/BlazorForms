using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using FormPortal.Core.Models;
using FormPortal.Core.Pdf;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;

namespace FormPortal.Components
{
    public partial class EntryList
    {
        [Parameter]
        public FormEntryFilter Filter { get; set; } = new();
        public int TotalItems { get; set; }
        public List<EntryListItem> Data { get; set; } = new();

        [Parameter, EditorRequired]
        public string NavUrl { get; set; } = string.Empty;
        [Parameter, EditorRequired]
        public string BaseUrl { get; set; } = string.Empty;
        public List<EntryListItem> DownloadingList { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo(BaseUrl);
            }

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

                string filename = item.Name;

                if (string.IsNullOrWhiteSpace(filename))
                {
                    filename = $"{item.FormName}_{item.EntryId}";
                }

                await downloadService.DownloadFile($"{filename}.pdf", data, "application/pdf");
            }
            DownloadingList.Remove(item);
        }
    }
}