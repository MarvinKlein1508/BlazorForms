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
using FluentValidation;

namespace BlazorForms.Components.Pages.Admin.Forms
{
    public partial class FormEditor : IAsyncDisposable
    {
        [Parameter]
        public int FormId { get; set; }
        public Form? Input { get; set; }

        public Form StartCopy { get; set; } = new();
        public List<FormElement> SelectedFormElementStack { get; set; } = [];
        public FormElement? SelectedFormElement { get; set; }
        public bool EditFormProperties { get; set; }
        public Guid? ScrollToGuid { get; set; }
        public string ContextMenuHeaderName { get; set; } = string.Empty;
        [Inject]
        public IValidator<Form> Validator { get; set; } = default!;
        public UserFilter FilterUser { get; set; } = new();

        private bool _showMobileToolbar;
        private bool _isToolbarDrag;
        private List<User> _searchUsers = [];
        private List<User> _searchManagers = [];
        private HotKeysContext _hotKeysContext = default!;


        private InputTagOptions _fileTypeOptions = new();

        public List<FormStatus> Statuses { get; set; } = [];

        private async Task<bool> ValidateFileTypeAsync(string fileType)
        {
            if (fileType.Contains('.'))
            {
                return false;
            }

            if (!Storage.MimeTypes.TryGetValue(fileType, out var _))
            {
                await JSRuntime.ShowToastAsync(ToastType.error, localizer["ERROR_INVALID_FILETYPE"]);
                return false;
            }

            return true;
        }
        protected override Task OnInitializedAsync()
        {
            _fileTypeOptions.InputPlaceholder = localizer["PLACEHOLDER_EXTENSIONS"];

            _hotKeysContext = hotKeys.CreateContext()
                .Add(ModCode.None, Code.Escape, new Func<Task>(async () =>
                {
                    await CloseItemAsync();
                    await InvokeAsync(StateHasChanged);
                }), new HotKeyOptions()
                {
                    Description = AppLocalizer["BACK"],
                    Exclude = Exclude.None
                })
                .Add(ModCode.Ctrl, Code.S, new Func<Task>(async () =>
                {
                    await SaveAsync();
                    await InvokeAsync(StateHasChanged);
                }), new HotKeyOptions()
                {
                    Description = AppLocalizer["SAVE"],
                    Exclude = Exclude.None
                });
            return base.OnInitializedAsync();
        }
        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = new MySqlController();
            Statuses = await FormStatusService.GetAllAsync(dbController);

            if (FormId > 0)
            {
                // This Task will take some time depending on the size of the form.
                // To not block the UI we run it in a different Task.
                await Task.Run(LoadEditModeAsync);

            }
            else
            {
                Input = new Form
                {
                    LanguageId = Storage.GetActiveLanguage().LanguageId,
                };
                Input.Rows.Add(new FormRow(Input, 1));
                EditFormProperties = true;
            }



            if (Input is not null)
            {
                if (Input.DefaultStatusId is 0)
                {
                    Input.DefaultStatusId = Statuses.First().StatusId;
                }

                StartCopy = Input.DeepCopyByExpressionTree();
            }
        }
        public async Task LoadEditModeAsync()
        {
            using IDbController dbController = new MySqlController();

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
            dragDropServiceRows.Items = [];

            dragDropServiceColumns.ActiveItem = null;
            dragDropServiceColumns.Items = [];

            dragDropServiceElements.ActiveItem = null;
            dragDropServiceElements.Items = [];

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
                dragDropServiceColumns.Items = [];
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
                dragDropServiceRows.Items = [];
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
            dragDropServiceElements.Items = [];
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
                var errorBuilder = new StringBuilder();
                errorBuilder.AppendLine(localizer["ERROR_VALIDATION"]);

                foreach (var item in validationResult.Errors)
                {
                    errorBuilder.AppendLine(Environment.NewLine);
                    errorBuilder.AppendLine(item.ErrorMessage);
                }

                string errorMessage = errorBuilder.ToString();

                await JSRuntime.ShowToastAsync(ToastType.error, errorMessage);
                return;
            }




            using IDbController dbController = new MySqlController();

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

                if (SelectedFormElementStack.Count != 0)
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

            await JSRuntime.ShowToastAsync(ToastType.success, localizer["MESSAGE_SAVED_SUCCESSFULLY"]);

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
            if (SelectedFormElement is not null || EditFormProperties)
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
                    await JSRuntime.ScrollToFragment(ScrollToGuid!.ToString()!, ScrollBehavior.auto);
                    ScrollToGuid = null;
                }
            }

        }
        private Task CloseItemAsync()
        {
            bool redirect = SelectedFormElement is null && !EditFormProperties;

            if (SelectedFormElement is not null)
            {
                SelectedFormElementStack.Remove(SelectedFormElement);

                if (SelectedFormElementStack.Count != 0)
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

            if (EditFormProperties)
            {
                EditFormProperties = false;
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
                using IDbController dbController = new MySqlController();
                if (searchManagers)
                {
                    FilterUser.BlockedIds = Input.ManagerUsers.Select(x => x.UserId).ToList();
                    _searchManagers = await userService.GetAsync(FilterUser, dbController);
                }
                else
                {
                    FilterUser.BlockedIds = Input.AllowedUsersForNewEntries.Select(x => x.UserId).ToList();
                    _searchUsers = await userService.GetAsync(FilterUser, dbController);
                }
            }
        }
        private Task UserSelectedAsync(User user, List<User> list)
        {
            _searchUsers.Remove(user);
            _searchManagers.Remove(user);

            var searchUser = list.FirstOrDefault(x => x.UserId == user.UserId);

            if (searchUser is null)
            {
                FilterUser.BlockedIds.Add(user.UserId);
                list.Add(user);
            }

            return Task.CompletedTask;
        }
        private Task UserRemovedAsync(User user, List<User> list)
        {

            list.Remove(user);
            FilterUser.BlockedIds.Remove(user.UserId);

            return Task.CompletedTask;
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            if (Input.HasBeenModified(StartCopy))
            {
                var isConfirmed = await JSRuntime.InvokeAsync<bool>("confirm", localizer["MESSAGE_UNSAVED_CHANGES"].Value);

                if (!isConfirmed)
                {
                    context.PreventNavigation();
                }
            }
        }


        public async ValueTask DisposeAsync()
        {
            await _hotKeysContext.DisposeAsync();
            GC.SuppressFinalize(_hotKeysContext);
        }
    }
}