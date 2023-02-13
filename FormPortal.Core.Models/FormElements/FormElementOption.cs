using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;

namespace FormPortal.Core.Models.FormElements
{
    public class FormElementOption : IDbModel
    {
        [CompareField("element_option_id")]
        public int ElementOptionId { get; set; }
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        public int Id => ElementOptionId;
        [CompareField("is_default_value")]
        public bool IsDefaultValue { get; set; }
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
