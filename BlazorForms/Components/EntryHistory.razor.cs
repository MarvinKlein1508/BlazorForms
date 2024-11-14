using BlazorForms.Core.Models;
using DbController;
using DbController.MySql;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components
{
    public partial class EntryHistory
    {
        [Parameter, EditorRequired]
        public int EntryId { get; set; }
        [Parameter]
        public string TableClass { get; set; } = "table table-bordered bg-white table-responsive-md";
        public List<FormEntryStatusChange> History { get; set; } = [];

        private bool _isLoading;
        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            _isLoading = true;
            await Task.Yield();
            using IDbController dbController = new MySqlController();
            History = await entryStatusService.GetHistoryAsync(EntryId, dbController);
            _isLoading = false;
        }
    }
}