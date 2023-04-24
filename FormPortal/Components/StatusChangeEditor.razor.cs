using Microsoft.AspNetCore.Components;
using FormPortal.Core.Services;
using DatabaseControllerProvider;
using FormPortal.Core.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace FormPortal.Components
{
    public partial class StatusChangeEditor
    {
        [Parameter, EditorRequired]
        public FormEntry? Entry { get; set; }
        [Parameter, EditorRequired]
        public User? User { get; set; }
        [Parameter, EditorRequired]
        public EventCallback<FormEntryStatusChange> OnSaved { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }

        public FormEntryStatusChange? Input { get; set; }
        private EditForm? _form;

        protected override Task OnParametersSetAsync()
        {
            ArgumentNullException.ThrowIfNull(User, nameof(User));
            ArgumentNullException.ThrowIfNull(Entry, nameof(Entry));

            Input = new()
            {
                UserId = User.Id,
                EntryId = Entry.Id
            };

            return base.OnParametersSetAsync();
        }

        private async Task SaveAsync()
        {
            if (Input is null || _form is null || _form.EditContext is null || Entry is null)
            {
                return;
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            await dbController.StartTransactionAsync();
            Input.DateAdded = DateTime.Now;

            if (_form.EditContext.Validate())
            {
                if (Input.Id is 0)
                {
                    await statusChangeService.CreateAsync(Input, dbController);
                }
                else
                {
                    await statusChangeService.UpdateAsync(Input, dbController);
                }

                // Update the status of the entry
                Entry.StatusId = Input.StatusId;
                





                await jsRuntime.ShowToastAsync(ToastType.success, "Datensatz erfolgreich gespeichert");

                await OnSaved.InvokeAsync(Input);
            }
        }
    }
}