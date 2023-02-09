using DatabaseControllerProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Models
{
    public class Form : IDbModel
    {
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("login_required")]
        public bool IsOnlyAvailableForLoggedInUsers { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; }
        public int Id => FormId;
        public List<FormRow> Rows { get; set; } = new();
        public virtual Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "FORM_ID", FormId },
                { "NAME", Name },
                { "LOGIN_REQUIRED", IsOnlyAvailableForLoggedInUsers },
                { "IS_ACTIVE", IsActive }
            };
        }

        public void RemoveEmptyRows()
        {
            var list = Rows.Where(x => !x.Columns.Any()).ToList();
            foreach (var item in list)
            {
                Rows.Remove(item);
            }
        }

        public void SetRowSortOrder()
        {
            int rowCount = 1;
            foreach (var row in Rows)
            {
                row.SortOrder = rowCount++;
            }
        }

        public IEnumerable<FormElement> GetElements()
        {
            foreach (var row in Rows)
            {
                foreach (var column in row.Columns)
                {
                    foreach (var element in column.Elements)
                    {
                        yield return element;
                    }
                }
            }
        }

        public IEnumerable<FormElement> GetRuleElements() => GetElements().Where(x => x.GetElementType() is ElementType.Checkbox or ElementType.Select or ElementType.Date or ElementType.Radio);

        public void DeleteRulesForElement(params FormElement[] elements)
        {
            foreach (var row in Rows)
            {
                foreach (var column in row.Columns)
                {
                    foreach (var element in column.Elements)
                    {
                        element.Rules.RemoveAll(x => elements.Contains(x.Element));
                    }
                }
            }
        }
    }
}
