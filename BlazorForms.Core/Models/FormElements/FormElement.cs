using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models.FormElements;

public abstract class FormElement : IDbModel<int?>, IHasSortableElement, IHasRuleSet, IHasTabs<FormElementTabs>, ICloneable
{
    public int ElementId { get; set; }
    public Guid Guid { get; set; }
    public int FormId { get; set; }
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public int TableParentElementId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsRequired { get; set; }
    public bool ResetOnCopy { get; set; }
    public RuleType RuleType { get; set; }
    public int SortOrder { get; set; }

    public int EntryId { get; set; }
    public int? GetIdentifier()
    {
        return ElementId > 0 ? ElementId : null;
    }
    public override string ToString() => Name;
    public abstract ElementType GetElementType();
    [IgnoreModificationCheck]
    public FormColumn? Parent { get; set; }
    public List<Rule> Rules { get; set; } = [];
    [IgnoreModificationCheck]
    public FormRow? Row => Parent?.Parent;
    [IgnoreModificationCheck]
    public Form? Form { get; set; }
    public virtual Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ELEMENT_ID", ElementId },
            { "FORM_ID", FormId },
            { "ROW_ID", RowId },
            { "GUID", Guid.ToString() },
            { "COLUMN_ID", ColumnId },
            { "NAME", Name },
            { "IS_ACTIVE", IsActive },
            { "IS_REQUIRED", IsRequired },
            { "RESET_ON_COPY", ResetOnCopy },
            { "SORT_ORDER", SortOrder },
            { "TYPE", GetElementType().ToString() },
            { "TABLE_PARENT_ELEMENT_ID", TableParentElementId },
            { "RULE_TYPE", RuleType.ToString() },
            { "ENTRY_ID", EntryId },
            { "VALUE_STRING", string.Empty },
            { "VALUE_BOOLEAN", false },
            { "VALUE_NUMBER", 0 },
            { "VALUE_DATE", null },
        };
    }

    /// <summary>
    /// Gets or sets a unique ID to identitfy this column within <see cref="FormTableElement.Elements"/>
    /// </summary>
    public Guid? GuidTableCount { get; set; }
    public FormElement()
    {
        GenerateGuid();
    }

    public void GenerateGuid()
    {
        Guid = Guid.NewGuid();
    }
    public abstract string GetDefaultName();
    [IgnoreModificationCheck]
    public FormElementTabs ActiveTab { get; set; }

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

        return Rules.ValidateRules();
    }

    public abstract void SetValue(FormEntryElement element);
    public abstract void Reset();

    public IEnumerable<FormElement> GetRuleElements()
    {
        ElementType[] allowedRuleTypes = [ElementType.Checkbox, ElementType.Date, ElementType.Number, ElementType.Radio, ElementType.Select];
        var elements = Form?.GetElements() ?? [];
        foreach (var element in elements)
        {
            if (element == this)
            {
                continue;
            }

            var elementType = element.GetElementType();

            if (elementType is ElementType.Table && element is FormTableElement formTableElement)
            {
                foreach (var tableElement in formTableElement.Elements)
                {
                    if (tableElement == this)
                    {
                        continue;
                    }

                    if (allowedRuleTypes.Contains(tableElement.GetElementType()))
                    {
                        yield return tableElement;
                    }
                }
            }
            else if (allowedRuleTypes.Contains(elementType))
            {
                yield return element;
            }
        }
    }

    public abstract object Clone();
}
