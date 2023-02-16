using BlazorContextMenu;
using DatabaseControllerProvider;
using FluentValidation.Results;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Services;
using FormPortal.Core.Validators.Admin;
using FormularPortal.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FormularPortal.Pages.Admin.Forms
{
    public partial class FormEditor
    {
        [Parameter]
        public int FormId { get; set; }
        public Form? Input { get; set; }

        public Form StartCopy { get; set; } = new();
        public List<FormElement> SelectedFormElementStack { get; set; } = new();
        public FormElement? SelectedFormElement { get; set; }
        public FormRow? SelectedFormRow { get; set; }

        public bool EditFormProperties { get; set; }

        public FormValidator Validator { get; } = new FormValidator(new FormRowValidator(new FormColumnValidator()));
        protected override async Task OnParametersSetAsync()
        {

            if (FormId > 0)
            {
                // This Task will take some time depending on the size of the form.
                // To not block the UI we run it in a different Task.
                await Task.Run(LoadEditModeAsync);

            }
            else
            {
                Input = new Form();
                Input.Rows.Add(new FormRow(Input, 1));
            }
        }

        public async Task LoadEditModeAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);

            Form? form = await formService.GetAsync(FormId, dbController);

            if (form is not null)
            {
                Input = form.DeepCopyByExpressionTree();
                StartCopy = form.DeepCopyByExpressionTree();
            }
        }
        public void DropDelete()
        {
            if (dragDropServiceRows.ActiveItem is not null)
            {
                // Delete all rules for each element in the row
                Input?.DeleteRulesForElement(dragDropServiceRows.ActiveItem.GetElements().ToArray());
                dragDropServiceRows.Items.Remove(dragDropServiceRows.ActiveItem);


            }
            else if (dragDropServiceColumns.ActiveItem is not null)
            {
                // Delete all rules for each element in the column
                Input?.DeleteRulesForElement(dragDropServiceColumns.ActiveItem.GetElements().ToArray());

                dragDropServiceColumns.Items.Remove(dragDropServiceColumns.ActiveItem);
                Input?.RemoveEmptyRows();
            }
            else if (dragDropServiceElements.ActiveItem is not null)
            {
                // Delete all rules for each element for this element
                Input?.DeleteRulesForElement(dragDropServiceElements.ActiveItem);
                dragDropServiceElements.Items.Remove(dragDropServiceElements.ActiveItem);
            }
        }
        public Task OnColumnDroppedAsync(FormColumn column, FormRow row)
        {
            column.Parent = row;
            Input?.RemoveEmptyRows();
            return Task.CompletedTask;
        }

        public Task OnElementDroppedAsync(FormElement element, FormColumn column)
        {
            element.Parent = column;
            return Task.CompletedTask;
        }


        public void StartDragColumnFromToolbar()
        {
            if (Input is not null)
            {
                dragDropServiceColumns.ActiveItem = new FormColumn(Input);
                dragDropServiceColumns.Items = new List<FormColumn>();
            }
            StateHasChanged();
        }
        public void StartDragRowFromToolbar()
        {
            if (Input is not null)
            {
                dragDropServiceRows.ActiveItem = new FormRow(Input, 1);
                dragDropServiceRows.Items = new List<FormRow>();
            }
            StateHasChanged();
        }
        public Task OnToolbarElementDragStartAsync(FormElement element)
        {
            var newElement = element.DeepCopyByExpressionTree();
            newElement.GenerateGuid();
            newElement.Form = Input;
            dragDropServiceElements.ActiveItem = newElement;
            dragDropServiceElements.Items = new List<FormElement>();
            StateHasChanged();
            return Task.CompletedTask;
        }


        public async Task SaveAsync()
        {
            if (Input is null)
            {
                return;
            }

            foreach (var element in Input.GetElements())
            {


                ValidationResult validationResult = Validator.Validate(Input);

                if (!validationResult.IsValid)
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, $"Speichern nicht möglich, da die Validierung des Formulars fehlgeschlagen ist.");
                    return;
                }

            }


            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);

            await dbController.StartTransactionAsync();

            try
            {
                if (Input.FormId is 0)
                {
                    await formService.CreateAsync(Input, dbController);

                }
                else
                {
                    await formService.UpdateAsync(Input, StartCopy, dbController);

                }

                await dbController.CommitChangesAsync();
            }
            catch (Exception)
            {
                await dbController.RollbackChangesAsync();
                throw;
            }

            if (FormId is 0)
            {
                navigationManager.NavigateTo($"/Admin/FormEditor/{Input.FormId}");
            }
            else
            {
                await OnParametersSetAsync();
            }

            await jsRuntime.ShowToastAsync(ToastType.success, "Form has been saved successfully.");

        }

        private Task OpenFormElementAsync(FormElement element)
        {
            SelectedFormElementStack.Add(element);
            SelectedFormElement = element;
            return Task.CompletedTask;
        }

        private Task CloseFormElementAsync()
        {
            if (SelectedFormElement is not null)
            {
                SelectedFormElementStack.Remove(SelectedFormElement);

                if (SelectedFormElementStack.Any())
                {
                    SelectedFormElement = SelectedFormElementStack.Last();
                }
                else
                {
                    SelectedFormElement = null;
                }
            }

            return Task.CompletedTask;
        }

        private string GetTabNavClass(bool isActive) => isActive ? "nav-link active" : "nav-link";
        public string GetTabClass(bool active) => active ? "tab-pane fade active show" : "tab-pane fade";

        private async Task UploadLogoAsync(InputFileChangeEventArgs e)
        {
            if (Input is null)
            {
                return;
            }
            await using MemoryStream fs = new();
            await e.File.OpenReadStream(e.File.Size).CopyToAsync(fs);

            Input.Logo = fs.ToArray();
        }

        private async Task UploadImageAsync(InputFileChangeEventArgs e)
        {
            if (Input is null)
            {
                return;
            }
            await using MemoryStream fs = new();
            await e.File.OpenReadStream(e.File.Size).CopyToAsync(fs);

            Input.Image = fs.ToArray();
        }

        private Task OnRowContextMenuDeleteAsync(ItemClickEventArgs e)
        {
            if (Input is not null)
            {
                var row = e.Data as FormRow;

                if (row is not null)
                {
                    Input.Rows.Remove(row);
                }
            }
            return Task.CompletedTask;
        }

        private Task OnRowContextMenuPropertiesAsync(ItemClickEventArgs e)
        {
            if (Input is not null && e.Data is FormRow row)
            {
                SelectedFormRow = row;
            }

            return Task.CompletedTask;
        }
    }
}