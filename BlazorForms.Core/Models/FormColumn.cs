using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models;

/// <summary>
/// Represents a column for a <see cref="FormRow"/>
/// </summary>
public class FormColumn : IDbModel<int?>, IHasSortableElement, IHasRuleSet, IHasTabs<FormColumnTabs>
{
    public int ColumnId { get; set; }
    public int FormId { get; set; }
    public int RowId { get; set; }
    public bool IsActive { get; set; } = true;
    public RuleType RuleType { get; set; }
    public int SortOrder { get; set; }
    public int? GetIdentifier()
    {
        return ColumnId > 0 ? ColumnId : null;
    }
    /// <summary>
    /// Gets or sets the elements for this column
    /// </summary>
    public List<FormElement> Elements { get; set; } = [];
    public List<Rule> Rules { get; set; } = [];
    [IgnoreModificationCheck]
    public FormRow? Parent { get; set; }
    [IgnoreModificationCheck]
    public Form? Form { get; set; }
    [IgnoreModificationCheck]
    public FormColumnTabs ActiveTab { get; set; }

    public FormColumn()
    {

    }

    public FormColumn(Form form)
    {
        Form = form;
    }
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "COLUMN_ID", ColumnId },
            { "FORM_ID", FormId },
            { "ROW_ID", RowId },
            { "IS_ACTIVE", IsActive},
            { "RULE_TYPE", RuleType.ToString() },
            { "SORT_ORDER", SortOrder }
        };
    }
    public IEnumerable<FormElement> GetElements()
    {
        foreach (var element in Elements)
        {
            yield return element;
        }
    }

    public bool IsVisible()
    {
        if (!IsActive)
        {
            return false;
        }

        if (RuleType is not RuleType.Visible and not RuleType.VisibleRequired)
        {
            return true;
        }

        if (Rules.Count == 0)
        {
            return true;
        }

        return Rules.ValidateRules() && Elements.Any(x => x.IsVisible());
    }

    public IEnumerable<FormElement> GetRuleElements()
    {
        ElementType[] allowedRuleTypes = [ElementType.Checkbox, ElementType.Date, ElementType.Number, ElementType.Radio, ElementType.Select];
        var elements = Form?.GetElements() ?? [];
        foreach (var element in elements)
        {
            var elementType = element.GetElementType();

            if (allowedRuleTypes.Contains(elementType))
            {
                yield return element;
            }
        }
    }
}
