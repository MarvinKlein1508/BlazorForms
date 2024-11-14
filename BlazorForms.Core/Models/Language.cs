using DbController;

namespace BlazorForms.Core.Models
{
    public class Language : IDbModelWithName<int?>
    {
        [CompareField("language_id")]
        public int LanguageId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("code")]
        public string Code { get; set; } = string.Empty;
        [CompareField("sort_order")]
        public int SortOrder { get; set; }
        [CompareField("status")]
        public bool Status { get; set; }

        public int? GetIdentifier()
        {
            return LanguageId > 0 ? LanguageId : null;
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object?> GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
