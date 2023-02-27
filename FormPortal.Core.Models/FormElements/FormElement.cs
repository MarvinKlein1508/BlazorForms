using DatabaseControllerProvider;
using FormPortal.Core.Constants;
using FormPortal.Core.Interfaces;

namespace FormPortal.Core.Models.FormElements
{
    public abstract class FormElement : IDbModel, IHasSortableElement, IHasRuleSet
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
        [CompareField("rule_type")]
        public RuleType RuleType { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int EntryId { get; set; }
        public int Id => ElementId;
        public override string ToString() => Name;
        public abstract ElementType GetElementType();
        public FormColumn? Parent { get; set; }
        public List<Rule> Rules { get; set; } = new();
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
                { "SORT_ORDER", SortOrder },
                { "TYPE", GetElementType() },
                { "TABLE_PARENT_ELEMENT_ID", TableParentElementId },
                { "RULE_TYPE", RuleType.ToString() },
                { "ENTRY_ID", EntryId },
                { "VALUE_STRING", string.Empty },
                { "VALUE_BOOLEAN", false },
                { "VALUE_NUMBER", 0 },
                { "VALUE_DATE", null },
            };
        }

        public FormElement()
        {
            GenerateGuid();
        }

        public void GenerateGuid()
        {
            Guid = Guid.NewGuid();
        }
        public abstract string GetDefaultName();

        public FormElementTabs ActiveTab { get; set; }

        public bool IsVisible()
        {
            if (!IsActive)
            {
                return false;
            }

            if (RuleType is not RuleType.Visible or RuleType.VisibleRequired)
            {
                return true;
            }

            if (!Rules.Any())
            {
                return true;
            }

            return Rules.ValidateRules();
        }
        
    }
    public enum ElementType
    {
        Checkbox = 0,
        Date = 1,
        File = 2,
        Label = 3,
        Number = 4,
        Radio = 5,
        Select = 6,
        Table = 7,
        Text = 8,
        Textarea = 9
    }

    public enum FormElementTabs
    {
        General,
        Rules,
        Elements,
        CalcSets
    }
}
