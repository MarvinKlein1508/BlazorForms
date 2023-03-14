using BlazorContextMenu;
using DatabaseControllerProvider;
using FluentValidation.Results;
using FormPortal.Core;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Services;
using FormPortal.Core.Validators.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;
namespace FormPortal.Pages.Admin.Forms
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
        public FormColumn? SelectedFormColumn { get; set; }
        public bool EditFormProperties { get; set; }
        public Guid? ScrollToGuid { get; set; }
        public string ContextMenuHeaderName { get; set; } = string.Empty;
        public FormValidator Validator { get; } = new FormValidator();
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
                EditFormProperties = true;
            }
        }

        public async Task LoadEditModeAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);

            Form? form = await formService.GetAsync(FormId, dbController);

            if (form is not null)
            {
                Input = form.DeepCopyByExpressionTree();
                StartCopy = form.DeepCopyByExpressionTree();
            }
        }
        public void DropDelete()
        {
            if (Input is null)
            {
                return;
            }

            if (dragDropServiceRows.ActiveItem is not null)
            {
                // Delete all rules for each element in the row
                Input.RemoveRow(dragDropServiceRows.ActiveItem);


            }
            else if (dragDropServiceColumns.ActiveItem is not null)
            {
                // Delete all rules for each element in the column
                Input.RemoveColumn(dragDropServiceColumns.ActiveItem);
                Input.RemoveEmptyRows();
            }
            else if (dragDropServiceElements.ActiveItem is not null)
            {
                // Delete all rules for each element for this element
                Input.DeleteRulesForElement(dragDropServiceElements.ActiveItem);
                dragDropServiceElements.Items.Remove(dragDropServiceElements.ActiveItem);
            }

            CleanToolbarDrag();
        }

        public void CleanToolbarDrag()
        {
            dragDropServiceRows.ActiveItem = null;
            dragDropServiceRows.Items = new List<FormRow>();

            dragDropServiceColumns.ActiveItem = null;
            dragDropServiceColumns.Items = new List<FormColumn>();

            dragDropServiceElements.ActiveItem = null;
            dragDropServiceElements.Items = new List<FormElement>();

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

            ValidationResult validationResult = Validator.Validate(Input);

            if (!validationResult.IsValid)
            {
                StringBuilder errorBuilder = new StringBuilder();
                errorBuilder.AppendLine("Speichern nicht möglich, da die Validierung des Formulars fehlgeschlagen ist.");

                foreach (var item in validationResult.Errors)
                {
                    errorBuilder.AppendLine(Environment.NewLine);
                    errorBuilder.AppendLine(item.ErrorMessage);
                }

                string errorMessage = errorBuilder.ToString();

                await jsRuntime.ShowToastAsync(ToastType.error, errorMessage);
                return;
            }




            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);

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

                // We need to reset the active object in order to save the correct data.
                if (SelectedFormRow is not null)
                {
                    SelectedFormRow = Input.Rows.FirstOrDefault(x => x.RowId == SelectedFormRow.RowId);
                }

                if (SelectedFormColumn is not null)
                {
                    SelectedFormColumn = Input.GetColumns().FirstOrDefault(x => x.RowId == SelectedFormColumn.RowId && x.ColumnId == SelectedFormColumn.ColumnId);
                }

                if (SelectedFormElement is not null)
                {
                    var activeTab = SelectedFormElement.ActiveTab;
                    SelectedFormElement = Input.GetAllElements().FirstOrDefault(x => x.ElementId == SelectedFormElement.ElementId);
                    if (SelectedFormElement is not null)
                    {
                        SelectedFormElement.ActiveTab = activeTab;
                    }
                }

                if (SelectedFormElementStack.Any())
                {
                    for (int i = 0; i < SelectedFormElementStack.Count;)
                    {
                        var element = Input.GetAllElements().FirstOrDefault(x => x.ElementId == SelectedFormElementStack[i].ElementId);

                        if (element is not null)
                        {
                            var activeTab = SelectedFormElementStack[i].ActiveTab;
                            element.ActiveTab = activeTab;
                            SelectedFormElementStack[i] = element;
                            i++;
                        }
                        else
                        {
                            SelectedFormElementStack.RemoveAt(i);
                            i--;
                        }
                    }

                }
            }

            await jsRuntime.ShowToastAsync(ToastType.success, "Form has been saved successfully.");

        }

        private Task OpenFormElementAsync(FormElement element)
        {
            SelectedFormElementStack.Add(element);
            SelectedFormElement = element;
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

        private Task OnContextMenuDeleteAsync(ItemClickEventArgs e)
        {
            if (Input is not null)
            {
                if (e.Data is FormRow row)
                {
                    Input.RemoveRow(row);
                }
                else if (e.Data is FormColumn column)
                {
                    Input.RemoveColumn(column);
                }
            }
            return Task.CompletedTask;
        }

        private Task OnContextMenuPropertiesAsync(ItemClickEventArgs e)
        {
            if (Input is not null)
            {
                if (e.Data is FormRow row)
                {
                    SelectedFormRow = row;
                }
                else if (e.Data is FormColumn column)
                {
                    SelectedFormColumn = column;
                }
            }

            return Task.CompletedTask;
        }

        private Task OnContextMenuAppearingAsync(MenuAppearingEventArgs e)
        {
            if (e.Data is FormRow row)
            {
                ContextMenuHeaderName = "Zeile";
            }
            else if (e.Data is FormColumn column)
            {
                ContextMenuHeaderName = "Spalte";
            }

            return Task.CompletedTask;
        }

        private string GetFormGridEditorCssClass()
        {
            if (SelectedFormColumn is not null || SelectedFormRow is not null || SelectedFormElement is not null)
            {
                return "d-none";
            }

            return string.Empty;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                if (ScrollToGuid is not null)
                {
                    await jsRuntime.ScrollToFragment(ScrollToGuid!.ToString()!, ScrollBehavior.auto);
                    ScrollToGuid = null;
                }
            }

        }

        private Task CloseItemAsync()
        {
            bool redirect = SelectedFormRow is null && SelectedFormColumn is null && SelectedFormElement is null;
            SelectedFormRow = null;
            SelectedFormColumn = null;
            if (SelectedFormElement is not null)
            {
                SelectedFormElementStack.Remove(SelectedFormElement);

                if (SelectedFormElementStack.Any())
                {
                    SelectedFormElement = SelectedFormElementStack.Last();
                }
                else
                {
                    // Cache element to jump back to it in editor
                    var tmp = SelectedFormElement;
                    ScrollToGuid = tmp.Guid;
                    SelectedFormElement = null;
                }
            }

            if (redirect)
            {
                navigationManager.NavigateTo("/Admin/Forms");
            }

            return Task.CompletedTask;
        }
    }
}