using BlazorInputTags;
using DbController;
using DbController.MySql;
using FluentValidation.Results;
using BlazorForms.Core;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;
using BlazorForms.Core.Services;
using BlazorForms.Core.Validators.Admin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Text;
using Toolbelt.Blazor.HotKeys2;

namespace BlazorForms.Pages.Admin.Forms
{
    public partial class FormEditor
    {
        [Parameter]
        public int FormId { get; set; }
        public Form? Input { get; set; }

        public Form StartCopy { get; set; } = new();
        public List<FormElement> SelectedFormElementStack { get; set; } = new();
        public FormElement? SelectedFormElement { get; set; }
        public bool EditFormProperties { get; set; }
        public Guid? ScrollToGuid { get; set; }
        public string ContextMenuHeaderName { get; set; } = string.Empty;
        public FormValidator Validator { get; } = new FormValidator();
        public UserFilter FilterUser { get; set; } = new();

        private bool _showMobileToolbar;
        private bool _isToolbarDrag;
        private List<User> _searchUsers = new();
        private List<User> _searchManagers = new();
#nullable disable
        private HotKeysContext _hotKeysContext;
#nullable enable

        private InputTagOptions _fileTypeOptions = new InputTagOptions
        {
            InputPlaceholder = "Enter extension, add with Enter"
        };

        public List<FormStatus> Statuses { get; set; } = new();

        private async Task<bool> ValidateFileTypeAsync(string fileType)
        {
            if (fileType.Contains('.'))
            {
                return false;
            }

            if (!AppdatenService.MimeTypes.TryGetValue(fileType, out var mimeType))
            {
                await jsRuntime.ShowToastAsync(ToastType.error, "Dateityp wird nicht unterstützt.");
                return false;
            }

            return true;
        }
        protected override Task OnInitializedAsync()
        {
            _hotKeysContext = hotKeys.CreateContext()
                .Add(ModCode.None, Code.Escape, async () =>
                {
                    await CloseItemAsync();
                    await InvokeAsync(StateHasChanged);
                }, "Zurück", Exclude.None)
                .Add(ModCode.Ctrl, Code.S, async () =>
                {
                    await SaveAsync();
                    await InvokeAsync(StateHasChanged);
                }, "Speichern", Exclude.None);
            return base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            Statuses = await FormStatusService.GetAllAsync(dbController);

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



            if (Input is not null)
            {
                if (Input.DefaultStatusId is 0)
                {
                    Input.DefaultStatusId = Statuses.First().Id;
                }

                StartCopy = Input.DeepCopyByExpressionTree();
            }
        }
        public async Task LoadEditModeAsync()
        {
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

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

            _isToolbarDrag = false;

        }
        private string GetToolbarDraggingCss()
        {
            if (dragDropServiceColumns.ActiveItem is not null || dragDropServiceElements.ActiveItem is not null || dragDropServiceRows.ActiveItem is not null)
            {
                return "plk-dd-in-transit no-pointer-events plk-dd-inprogess";
            }

            return string.Empty;
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
                _showMobileToolbar = false;
                _isToolbarDrag = true;
            }
            StateHasChanged();
        }
        public void StartDragRowFromToolbar()
        {
            if (Input is not null)
            {
                dragDropServiceRows.ActiveItem = new FormRow(Input, 1);
                dragDropServiceRows.Items = new List<FormRow>();
                _showMobileToolbar = false;
                _isToolbarDrag = true;
            }
            StateHasChanged();
        }
        public void OnToolbarElementDragStart(FormElement element)
        {
            var newElement = element.DeepCopyByExpressionTree();
            newElement.GenerateGuid();
            newElement.Form = Input;
            dragDropServiceElements.ActiveItem = newElement;
            dragDropServiceElements.Items = new List<FormElement>();
            _showMobileToolbar = false;
            _isToolbarDrag = true;
            StateHasChanged();
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




            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

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
        private string GetDeleteWrapperClass()
        {
            if (!_isToolbarDrag && (dragDropServiceColumns.ActiveItem is not null || dragDropServiceElements.ActiveItem is not null || dragDropServiceRows.ActiveItem is not null))
            {
                return "d-block";
            }

            return "d-none";
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
        private string GetFormGridEditorCssClass()
        {
            if (SelectedFormElement is not null)
            {
                return "d-none";
            }

            return string.Empty;
        }
        private string GetToobalWrapperCss()
        {
            if (_showMobileToolbar)
            {
                return "d-block";
            }
            else
            {
                return "d-none";
            }
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
            bool redirect = SelectedFormElement is null;

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
        private async Task PerformSearch(bool searchManagers = false)
        {
            if (Input is not null)
            {
                using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
                if (searchManagers)
                {
                    FilterUser.BlockedIds = Input.ManagerUsers.Select(x => x.Id).ToList();
                    _searchManagers = await userService.GetAsync(FilterUser, dbController);
                }
                else
                {
                    FilterUser.BlockedIds = Input.AllowedUsersForNewEntries.Select(x => x.Id).ToList();
                    _searchUsers = await userService.GetAsync(FilterUser, dbController);
                }
            }
        }
        private Task UserSelectedAsync(User user, List<User> list)
        {
            _searchUsers.Remove(user);
            _searchManagers.Remove(user);

            var searchUser = list.FirstOrDefault(x => x.Id == user.Id);

            if (searchUser is null)
            {
                FilterUser.BlockedIds.Add(user.Id);
                list.Add(user);
            }

            return Task.CompletedTask;
        }
        private Task UserRemovedAsync(User user, List<User> list)
        {

            list.Remove(user);
            FilterUser.BlockedIds.Remove(user.Id);

            return Task.CompletedTask;
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            if (Input.HasBeenModified(StartCopy))
            {
                var isConfirmed = await jsRuntime.InvokeAsync<bool>("confirm", "Sie haben noch nicht alle Änderungen gespeichert. Möchten Sie den Editor dennoch verlassen?");

                if (!isConfirmed)
                {
                    context.PreventNavigation();
                }
            }
        }

        public void Dispose()
        {
            _hotKeysContext.Dispose();
        }
    }
}