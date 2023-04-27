using DbController;
using DbController.MySql;
using FormPortal.Core.Models;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;

namespace FormPortal.Components
{
    public partial class EntryHistory
    {
        [Parameter, EditorRequired]
        public int EntryId { get; set; }
        [Parameter]
        public string TableClass { get; set; } = "table table-bordered bg-white table-responsive-md";
        public List<FormEntryStatusChange> History { get; set; } = new();

        private bool _isLoading;
        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            _isLoading = true;
            await Task.Yield();
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            History = await entryStatusService.GetHistoryAsync(EntryId, dbController);
            _isLoading = false;
        }
    }
}