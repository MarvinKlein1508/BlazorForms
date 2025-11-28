using BlazorForms.Domain.Entities;
using BlazorForms.Domain.Entities.Elements;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorForms.Web.Components;

public partial class FormEditor
{
    private bool _dragFromToolbar;
    private bool _editFormProperties { get; set; }
    private Guid? _scrollToGuid;
    private FormElementBase? _selectedFormElement;
    private FormRow? _activeDragFormRow;
    private FormColumn? _activeDragFormColumn;
    private FormElementBase? _activeDragFormElement;
    private FormRow _toolbarRow = new();
    private FormColumn? _toolbarColumn;

    private readonly List<FormElementBase> _selectedFormElementStack = [];
    public static List<FormElementBase> ToolbarElements { get; } =
    [
        new FormTextElement(),
        new FormTextAreaElement(),
        new FormLabelElement(),
        new FormCheckboxElement(),
        new FormRadioElement(),
        new FormSelectElement(),
        new FormDateElement(),
    ];

    [Parameter, EditorRequired]
    public EventCallback OnSave { get; set; }

    [Parameter, EditorRequired]
    public Form? Input { get; set; }
    private void OnRowDropEnd(FluentDragEventArgs<FormRow> e)
    {
        if (Input is null)
        {
            return;
        }

        var target = e.Target.Item;
        var source = e.Source.Item;

        if (_dragFromToolbar)
        {
            source = new FormRow(Input, 1);
        }

        int targetIndex = Input.Rows.IndexOf(target);

        if (targetIndex < 0)
        {
            targetIndex = 0;
        }

        if (!_dragFromToolbar)
        {
            Input.Rows.Remove(source);
        }

        Input.Rows.Insert(targetIndex, source);
        CleanToolbarDrag();
    }

    private void OnColumnDropEnd(FluentDragEventArgs<FormColumn> e)
    {
        if (e.Source.Data is not FormRow sourceRow || e.Target.Data is not FormRow targetRow)
        {
            return;
        }

        var target = e.Target.Item;
        var source = e.Source.Item;
        int targetIndex = targetRow.Columns.IndexOf(target);

        if (sourceRow == targetRow)
        {
            sourceRow.Columns.Remove(source);
            sourceRow.Columns.Insert(targetIndex, source);
        }
        else
        {
            sourceRow.Columns.Remove(source);
            if (targetIndex != -1)
            {
                targetRow.Columns.Insert(targetIndex, source);
            }
            else
            {
                targetRow.Columns.Add(source);
            }
        }

        StateHasChanged();
        CleanToolbarDrag();
    }

    private void OnDropElement(FluentDragEventArgs<FormElementBase> e)
    {
        var sourceColumn = e.Source.Data as FormColumn;

        if (e.Target.Data is not FormColumn targetColumn)
        {
            return;
        }

        var source = e.Source.Item;
        var target = e.Target.Item;

        int targetIndex = targetColumn.Elements.IndexOf(target);

        if (sourceColumn == targetColumn)
        {
            sourceColumn.Elements.Remove(source);
            sourceColumn.Elements.Insert(targetIndex, source);
        }
        else
        {
            if (sourceColumn is not null && !_dragFromToolbar)
            {
                sourceColumn.Elements.Remove(source);
            }

            if (targetIndex != -1)
            {
                targetColumn.Elements.Insert(targetIndex, source);
            }
            else
            {
                targetColumn.Elements.Add(source);
            }
        }

        _dragFromToolbar = false;
        StateHasChanged();
        CleanToolbarDrag();
    }

    private Task OpenFormElementAsync(FormElementBase element)
    {
        _selectedFormElementStack.Add(element);
        _selectedFormElement = element;
        return Task.CompletedTask;
    }

    private Task CloseItemAsync()
    {
        bool redirect = _selectedFormElement is null && !_editFormProperties;

        if (_selectedFormElement is not null)
        {
            _selectedFormElementStack.Remove(_selectedFormElement);

            if (_selectedFormElementStack.Count != 0)
            {
                _selectedFormElement = _selectedFormElementStack.Last();
            }
            else
            {
                // Cache element to jump back to it in editor
                var tmp = _selectedFormElement;
                _scrollToGuid = tmp.Guid;
                _selectedFormElement = null;
            }
        }

        if (_editFormProperties)
        {
            _editFormProperties = false;
        }

        //if (redirect)
        //{
        //    navigationManager.NavigateTo("/Admin/Forms");
        //}

        return Task.CompletedTask;
    }

    private string GetDeleteWrapperClass()
    {
        if (!_dragFromToolbar && (_activeDragFormRow is not null || _activeDragFormColumn is not null || _activeDragFormElement is not null))
        {
            return "d-block";
        }

        return "d-none";
    }

    public void DropDelete()
    {
        if (Input is null)
        {
            return;
        }

        if (_activeDragFormRow is not null)
        {
            // Delete all rules for each element in the row
            Input.RemoveRow(_activeDragFormRow);
        }
        else if (_activeDragFormColumn is not null)
        {
            // Delete all rules for each element in the column
            Input.RemoveColumn(_activeDragFormColumn);
            Input.RemoveEmptyRows();
        }
        else if (_activeDragFormElement is not null)
        {
            // Delete all rules for each element for this element
            //_testForm.DeleteRulesForElement(dragDropServiceElements.ActiveItem);
            Input.RemoveElement(_activeDragFormElement);
        }

        CleanToolbarDrag();
    }

    public void CleanToolbarDrag()
    {
        _activeDragFormRow = null;
        _activeDragFormColumn = null;
        _activeDragFormElement = null;
        _dragFromToolbar = false;
    }

    private async void OnRowDragStart(FluentDragEventArgs<FormRow> e)
    {
        _activeDragFormRow = e.Source.Item;
        await InvokeAsync(StateHasChanged);
    }

    private async void OnColumnDragStart(FluentDragEventArgs<FormColumn> e)
    {
        _activeDragFormColumn = e.Source.Item;
        await InvokeAsync(StateHasChanged);
    }

    private async void OnElementDragStart(FluentDragEventArgs<FormElementBase> e)
    {
        _activeDragFormElement = e.Source.Item;
        await InvokeAsync(StateHasChanged);
    }
    public void StartDragRowFromToolbar()
    {
        if (Input is not null)
        {
            _toolbarRow = new FormRow(Input, 1);
            _dragFromToolbar = true;
        }

        StateHasChanged();
    }
    public void StartDragColumnFromToolbar()
    {
        if (Input is not null)
        {
            _toolbarColumn = new FormColumn(Input);
            _dragFromToolbar = true;
        }

        StateHasChanged();
    }
}
