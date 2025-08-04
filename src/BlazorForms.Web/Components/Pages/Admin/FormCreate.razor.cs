using BlazorForms.Application.Domain;
using BlazorForms.Application.Domain.Elements;
using Microsoft.FluentUI.AspNetCore.Components;
using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace BlazorForms.Web.Components.Pages.Admin;
public partial class FormCreate
{
    private bool _dragFromToolbar;
    public static List<FormElementBase> ToolbarElements { get; } =
    [
        new FormTextElement(),
        new FormTextAreaElement(),
        new FormLabelElement()
    ];

    private Form _testForm;
    public FormCreate()
    {
        _testForm = new Form();

        var rows = Enumerable.Range(1, 6)
                             .Select(id => new FormRow { RowId = id })
                             .ToList();

        var columns = Enumerable.Range(1, 9)
                                .Select(id => new FormColumn { ColumnId = id })
                                .ToList();

        var elementMap = new Dictionary<int, int>
        {
            { 1, 2 },
            { 2, 1 },
            { 3, 3 },
            { 4, 0 },
            { 5, 2 },
            { 6, 1 },
            { 7, 0 },
            { 8, 1 },
            { 9, 0 }
        };

        int elementIdCounter = 1;
        foreach (var column in columns)
        {
            if (elementMap.TryGetValue(column.ColumnId, out int count))
            {
                for (int i = 0; i < count; i++)
                {
                    column.Elements.Add(new FormTextElement
                    {
                        ElementId = elementIdCounter++
                    });
                }
            }
        }

        rows[0].Columns.AddRange([columns[0], columns[1]]);
        rows[1].Columns.Add(columns[2]);
        rows[2].Columns.Add(columns[3]);
        rows[3].Columns.AddRange([columns[4], columns[5]]);
        rows[4].Columns.AddRange([columns[6], columns[7]]);
        rows[5].Columns.Add(columns[8]);

        _testForm.Rows.AddRange(rows);
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
    }

    private void OnDropElement(FluentDragEventArgs<FormElementBase> e)
    {
        var sourceColumn = e.Source.Data as FormColumn;
        var targetColumn = e.Target.Data as FormColumn;

        if (targetColumn is null)
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
    }
}