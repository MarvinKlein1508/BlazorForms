using DbController;
using DbController.MySql;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Pdf;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using BlazorBootstrap;
using BlazorForms.Core;
using BlazorForms.Core.Enums;

namespace BlazorForms.Components
{
    public partial class EntryList
    {
        private ConfirmDialog _deleteModal = default!;
        [Parameter]
        public FormEntryFilter DefaultFilter { get; set; } = new()
        {
            Limit = AppdatenService.PageLimit
        };
        public FormEntryFilter? Filter { get; set; }

        public FormEntryFilter? SavedFilter { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (double)(Filter?.Limit ?? DefaultFilter.Limit));
        public List<EntryListItem> Data { get; set; } = [];

        [Parameter, EditorRequired]
        public string NavUrl { get; set; } = string.Empty;
        [Parameter, EditorRequired]
        public string BaseUrl { get; set; } = string.Empty;
        [Parameter, EditorRequired]
        public int CurrentPage { get; set; }
        public List<EntryListItem> DownloadingList { get; set; } = [];

        public bool UserCanDeleteEntries { get; set; }
        public User? User { get; set; }

        private List<FormStatus> Statuses { get; set; } = [];

        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            User = await authService.GetUserAsync(dbController);

            if (User is not null)
            {
                Filter = await savedFilterService.GetAsync(DefaultFilter.DeepCopyByExpressionTree(), User.Id, BaseUrl, dbController);
            }

            if (Filter is not null)
            {
                Filter.PageNumber = CurrentPage;
            }
            await LoadAsync();
            UserCanDeleteEntries = await authService.HasRole(Roles.DELETE_ENTRIES);

        }

        private Task OnPageChangedAsync(int pageNumber)
        {
            navigationManager.NavigateTo($"{NavUrl}{pageNumber}");
            return Task.CompletedTask;
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (Filter is null)
            {
                return;
            }

            if (navigateToPage1)
            {
                navigationManager.NavigateTo(BaseUrl);
            }

            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

            Statuses = await FormStatusService.GetAllAsync(dbController);
            TotalItems = await formEntryService.GetTotalAsync(Filter, dbController);
            Data = await formEntryService.GetAsync(Filter, dbController);
            if (User is not null)
            {
                var savedFilter = Filter.ToSavedFilter<FormEntryFilter>(User.Id, BaseUrl);
                await savedFilterService.SaveAsync(savedFilter, dbController);
            }
        }

        private async Task DownloadAsync(EntryListItem item)
        {
            DownloadingList.Add(item);
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
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

        private async Task ShowDeleteModalAsync(EntryListItem input)
        {

            var options = new ConfirmDialogOptions
            {
                YesButtonText = appLocalizer["YES"],
                YesButtonColor = ButtonColor.Success,
                NoButtonText = appLocalizer["NO"],
                NoButtonColor = ButtonColor.Danger
            };

            var confirmation = await _deleteModal.ShowAsync(
            title: localizer["MODAL_DELETE_TITLE"],
            message1: String.Format(localizer["MODAL_DELETE_TEXT"], input.Name),
            confirmDialogOptions: options);

            if (confirmation)
            {
                using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

                await dbController.StartTransactionAsync();

                try
                {
                    await formEntryService.DeleteAsync(input, dbController);
                    await dbController.CommitChangesAsync();
                    await jsRuntime.ShowToastAsync(ToastType.success, localizer["MODAL_DELETE_SUCCESS"]);
                }
                catch (Exception)
                {
                    await dbController.RollbackChangesAsync();
                    throw;
                }

                await LoadAsync();
            }
        }

        private bool CanDeleteEntry(EntryListItem entry)
        {
            if (UserCanDeleteEntries)
            {
                return true;
            }

            if (User is null)
            {
                return false;
            }

            return entry.CreationUserId == User.UserId || entry.ManagerIds.Contains(User.UserId);
        }

        private async Task ResetFilterAsync()
        {
            Filter = DefaultFilter.DeepCopyByExpressionTree();
            await LoadAsync(true);
        }

        private string GetPriority(Priority priority) => priority switch
        {
            Priority.Low => (string)localizer["PRIORITY_LOW"],
            Priority.Normal => (string)localizer["PRIORITY_NORMAL"],
            Priority.High => (string)localizer["PRIORITY_HIGH"],
            _ => (string)appLocalizer["UNKOWN"],
        };
    }
}