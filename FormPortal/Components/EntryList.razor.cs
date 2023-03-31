using DatabaseControllerProvider;
using FormPortal.Core.Constants;
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
        public FormEntryFilter Filter { get; set; } = new()
        {
            Limit = AppdatenService.PageLimit
        };
        public int TotalItems { get; set; }
        public List<EntryListItem> Data { get; set; } = new();

        [Parameter, EditorRequired]
        public string NavUrl { get; set; } = string.Empty;
        [Parameter, EditorRequired]
        public string BaseUrl { get; set; } = string.Empty;
        public List<EntryListItem> DownloadingList { get; set; } = new();

        public bool UserCanDeleteEntries { get; set; }
        public User? User { get; set; }
        public EntryListItem? SelectedForDeletion { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
            UserCanDeleteEntries = await authService.HasRole(Roles.DELETE_ENTRIES);
            User = await authService.GetUserAsync();
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo(BaseUrl);
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            TotalItems = await formEntryService.GetTotalAsync(Filter, dbController);
            Data = await formEntryService.GetAsync(Filter, dbController);
        }

        private async Task DownloadAsync(EntryListItem item)
        {
            DownloadingList.Add(item);
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
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

        private async Task DeleteAsync()
        {
            if (SelectedForDeletion is null)
            {
                return;
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);

            await dbController.StartTransactionAsync();

            try
            {

                await formEntryService.DeleteAsync(SelectedForDeletion, dbController);
                await dbController.CommitChangesAsync();
                await jsRuntime.ShowToastAsync(ToastType.success, "Formulareintrag wurde erfolgreich gelöscht.");


                SelectedForDeletion = null;
            }
            catch (Exception)
            {
                await dbController.RollbackChangesAsync();
                throw;
            }

            await LoadAsync();
        }

        private bool CanDeleteEntry(EntryListItem entry)
        {
            if (UserCanDeleteEntries)
            {
                return true;
            }
            
            if(User is null)
            {
                return false;
            }

            return entry.CreationUserId == User.UserId || entry.ManagerIds.Contains(User.UserId);
        }
    }
}