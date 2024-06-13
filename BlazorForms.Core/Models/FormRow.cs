using DbController;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models
{
    /// <summary>
    /// Represents a row within the Form.
    /// </summary>
    public class FormRow : IDbModel, IHasSortableElement, IHasRuleSet, IHasTabs<FormRowTabs>
    {
        [CompareField("row_id")]
        public int RowId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; } = true;
        [CompareField("rule_type")]
        public RuleType RuleType { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public int Id => RowId;
        /// <summary>
        /// Gets or sets all columns for this row.
        /// </summary>
        public List<FormColumn> Columns { get; set; } = [];
        public List<Rule> Rules { get; set; } = [];
        [IgnoreModificationCheck]
        public Form? Form { get; set; }
        [IgnoreModificationCheck]
        public FormRowTabs ActiveTab { get; set; }

        /// <summary>
        /// Creates an empty row.
        /// </summary>
        public FormRow()
        {

        }
        /// <summary>
        /// Creates a new row with a specified amount of columns.
        /// </summary>
        /// <param name="columns"></param>
        public FormRow(Form form, int columns)
        {
            Form = form;
            for (int i = 0; i < columns; i++)
            {
                var column = new FormColumn(form)
                {
                    Parent = this
                };
                Columns.Add(column);
            }
        }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "ROW_ID", RowId },
                { "FORM_ID", FormId },
                { "IS_ACTIVE", IsActive },
                { "RULE_TYPE", RuleType.ToString() },
                { "SORT_ORDER", SortOrder }
            };
        }



        public IEnumerable<FormElement> GetElements()
        {
            foreach (var column in Columns)
            {
                foreach (var element in column.Elements)
                {
                    yield return element;
                }
            }
        }

        public string GetColumnClass()
        {
            if (Columns.Count is 1)
            {
                return "col-12";
            }

            if (Columns.Count is 2)
            {
                return "col-md-6";
            }

            if (Columns.Count is 3)
            {
                return "col-md-4";
            }

            if (Columns.Count is 4)
            {
                return "col-md-3";
            }

            return "col";
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

            return Rules.ValidateRules() && Columns.Any(x => x.IsVisible());
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
}
