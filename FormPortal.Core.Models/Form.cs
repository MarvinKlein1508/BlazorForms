using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    public class Form : IDbModel, IHasSortableElement
    {
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("description")]
        public string Description { get; set; } = string.Empty;
        [CompareField("login_required")]
        public bool IsOnlyAvailableForLoggedInUsers { get; set; }
        [CompareField("is_active")]
        public bool IsActive { get; set; }
        [CompareField("logo")]
        public byte[] Logo { get; set; } = Array.Empty<byte>();

        [CompareField("image")]
        public byte[] Image { get; set; } = Array.Empty<byte>();
        [CompareField("sort_order")]
        public int SortOrder { get; set; }
        public int Id => FormId;
        public List<FormRow> Rows { get; set; } = new();
        public virtual Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "FORM_ID", FormId },
                { "NAME", Name },
                { "DESCRIPTION", Description },
                { "LOGO", Logo },
                { "IMAGE", Image },
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

        public IEnumerable<FormNumberElement> GetCalcRuleSetElements()
        {
            foreach (var element in GetElements())
            {
                if(element is FormTableElement formTableElement)
                {
                    foreach (var table_element in formTableElement.Elements)
                    {
                        if (table_element is FormNumberElement formNumberElement)
                        {
                            yield return formNumberElement;
                        }
                    }
                }
                else if(element is FormNumberElement formNumberElement)
                {
                    yield return formNumberElement;
                }
            }
        }
        public void DeleteRulesForElement(params FormElement[] elements)
        {
            foreach (var row in Rows)
            {
                row.Rules.RemoveAll(x => elements.Contains(x.Element));
                foreach (var column in row.Columns)
                {
                    column.Rules.RemoveAll(x => elements.Contains(x.Element));
                    foreach (var element in column.Elements)
                    {
                        element.Rules.RemoveAll(x => elements.Contains(x.Element));

                        if(element is FormTableElement formTableElement)
                        {
                            foreach (var table_element in formTableElement.Elements)
                            {
                                table_element.Rules.RemoveAll(x => elements.Contains(x.Element));
                            }
                        }
                    }
                }
            }
        }
    }
}
