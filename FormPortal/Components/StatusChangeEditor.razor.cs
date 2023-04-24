using Microsoft.AspNetCore.Components;
using FormPortal.Core.Services;
using DatabaseControllerProvider;
using FormPortal.Core.Models;

namespace FormPortal.Components
{
    public partial class StatusChangeEditor
    {
        [Parameter, EditorRequired]
        public FormEntryStatusChange? Input { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnSaved { get; set; }

        private async Task SaveAsync()
        {
            if(Input is null)
            {
                return;
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            Input.DateAdded = DateTime.Now;

            if (Input.Id is 0)
            {
                await formEntryStatusChangeService.CreateAsync(Input, dbController);
            }
            else
            {
                await formEntryStatusChangeService.UpdateAsync(Input, dbController);
            }
        }
    }
}