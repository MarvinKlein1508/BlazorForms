using DbController;

namespace BlazorForms.Core.Models.FormElements
{
    public class FormElementOption : IDbModel<int?>
    {
        [CompareField("element_option_id")]
        public int ElementOptionId { get; set; }
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("is_default_value")]
        public bool IsDefaultValue { get; set; }
        public int? GetIdentifier()
        {
            return ElementOptionId > 0 ? ElementOptionId : null;
        }
        public virtual Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "ELEMENT_OPTION_ID", ElementOptionId },
                { "ELEMENT_ID", ElementId },
                { "NAME", Name },
                { "IS_DEFAULT_VALUE", IsDefaultValue },
            };
        }
    }
}
