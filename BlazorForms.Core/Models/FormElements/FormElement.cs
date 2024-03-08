using DbController;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models.FormElements
{
    public abstract class FormElement : IDbModel, IHasSortableElement, IHasRuleSet, IHasTabs<FormElementTabs>, ICloneable
    {
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("guid")]
        public Guid Guid { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("column_id")]
        public int ColumnId { get; set; }
        [CompareField("table_parent_element_id")]
        public int TableParentElementId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("is_active")]
        public bool IsActive { get; set; } = true;
        [CompareField("is_required")]
        public bool IsRequired { get; set; }
        [CompareField("reset_on_copy")]
        public bool ResetOnCopy { get; set; }
        [CompareField("rule_type")]
        public RuleType RuleType { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int EntryId { get; set; }
        public int Id => ElementId;
        public override string ToString() => Name;
        public abstract ElementType GetElementType();
        [IgnoreModificationCheck]
        public FormColumn? Parent { get; set; }
        public List<Rule> Rules { get; set; } = new();
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

            if (!Rules.Any())
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
            var elements = Form?.GetElements() ?? Enumerable.Empty<FormElement>();
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
}
