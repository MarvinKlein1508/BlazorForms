using BlazorForms.Application.Domain;
using BlazorForms.Application.Domain.Elements;
using Microsoft.FluentUI.AspNetCore.Components;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace BlazorForms.Web.Components.Pages.Admin;
public partial class FormCreate
{
    private Form _testForm;
    public FormCreate()
    {
        _testForm = new Form();

        var testRow1 = new FormRow() { RowId = 1 };
        var testRow2 = new FormRow() { RowId = 2 };
        var testRow3 = new FormRow() { RowId = 3 };
        var testRow4 = new FormRow() { RowId = 4 };
        var testRow5 = new FormRow() { RowId = 5 };
        var testRow6 = new FormRow() { RowId = 6 };
        var testColumn1 = new FormColumn() { ColumnId = 1 };
        var testColumn2 = new FormColumn() { ColumnId = 2 };
        var testColumn3 = new FormColumn() { ColumnId = 3 };
        var testColumn4 = new FormColumn() { ColumnId = 4 };
        var testColumn5 = new FormColumn() { ColumnId = 5 };
        var testColumn6 = new FormColumn() { ColumnId = 6 };
        var testColumn7 = new FormColumn() { ColumnId = 7 };
        var testColumn8 = new FormColumn() { ColumnId = 8 };
        var testElement1 = new FormTextElement() { ElementId = 1 };
        var testElement2 = new FormTextElement() { ElementId = 2 };
        var testElement3 = new FormTextElement() { ElementId = 3 };
        var testElement4 = new FormTextElement() { ElementId = 4 };
        var testElement5 = new FormTextElement() { ElementId = 5 };
        var testElement6 = new FormTextElement() { ElementId = 6 };
        var testElement7 = new FormTextElement() { ElementId = 7 };
        var testElement8 = new FormTextElement() { ElementId = 8 };
        var testElement9 = new FormTextElement() { ElementId = 9 };
        var testElement10 = new FormTextElement() { ElementId = 10 };

        testColumn1.Elements.AddRange(testElement1, testElement2);
        testColumn2.Elements.AddRange(testElement3, testElement4, testElement5);
        testColumn4.Elements.AddRange(testElement6);
        testColumn5.Elements.AddRange(testElement7, testElement8);
        testColumn6.Elements.AddRange(testElement9, testElement10);

        testRow1.Columns.AddRange(testColumn1, testColumn2);
        testRow2.Columns.Add(testColumn3);
        testRow3.Columns.Add(testColumn4);
        testRow4.Columns.AddRange(testColumn5, testColumn6);
        testRow5.Columns.AddRange(testColumn7, testColumn8);
        _testForm.Rows.AddRange(testRow1, testRow2, testRow3, testRow4, testRow5, testRow6);
    }
    private static Icon HomeIcon(bool active = false) =>
        active ? new Icons.Filled.Size24.Home()
               : new Icons.Regular.Size24.Home();

    private static Icon AppBarIcon(bool active = false) =>
        active ? new Icons.Filled.Size24.AppsList()
               : new Icons.Regular.Size24.AppsList();

    private static Icon WhatsNewIcon(bool active = false) =>
        active ? new Icons.Filled.Size24.Info()
               : new Icons.Regular.Size24.Info();

    private static Icon IconsIcon(bool active = false) =>
        active ? new Icons.Filled.Size24.Symbols()
               : new Icons.Regular.Size24.Symbols();

    private static Icon DialogIcon(bool active = false) =>
        active ? new Icons.Filled.Size24.AppGeneric()
               : new Icons.Regular.Size24.AppGeneric();


    private void HandleOnClick(IAppBarItem item)
    {


    }

    private async Task ShowSuccessAsync(IAppBarItem item)
    {
        var dialog = await DialogService.ShowSuccessAsync($"You clicked {item.Text}");
        var result = await dialog.Result;
    }

    private async Task ShowWarningAsync(IAppBarItem item)
    {
        var dialog = await DialogService.ShowWarningAsync($"Are you sure? {item.Text}");
        var result = await dialog.Result;
    }

    private void OnRowDropEnd(FluentDragEventArgs<FormRow> e)
    {
        var target = e.Target.Item;
        var source = e.Source.Item;

        int targetIndex = _testForm.Rows.IndexOf(target);

        _testForm.Rows.Remove(source);
        _testForm.Rows.Insert(targetIndex, source);
    }

    private void OnColumnDropEnd(FluentDragEventArgs<FormColumn> e)
    {
        var sourceRow = e.Source.Data as FormRow;
        var targetRow = e.Target.Data as FormRow;

        if (sourceRow is null || targetRow is null)
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
            targetRow.Columns.Insert(targetIndex, source);
        }

        StateHasChanged();
    }
}