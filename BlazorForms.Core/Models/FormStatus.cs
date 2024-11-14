using DbController;
using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models
{
    public class FormStatus : LocalizationModelBase<FormStatusDescription>, ILocalizedDbModel
    {
        [CompareField("status_id")]
        public int UserFilterId { get; set; }
        [CompareField("requires_approval")]
        public bool RequiresApproval { get; set; }
        [CompareField("is_completed")]
        public bool IsCompleted { get; set; }
        [CompareField("sort_order")]
        public int SortOrder { get; set; }

        public IEnumerable<Dictionary<string, object?>> GetLocalizedParameters()
        {
            foreach (var item in Description)
            {
                yield return new Dictionary<string, object?>
                {
                    { "STATUS_ID", UserFilterId },
                    { "CODE", item.Code },
                    { "NAME", item.Name },
                    { "DESCRIPTION", item.Description }
                };
            }
        }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "STATUS_ID", UserFilterId },
                { "REQUIRES_APPROVAL", RequiresApproval },
                { "IS_COMPLETED", IsCompleted },
                { "SORT_ORDER", SortOrder }
            };
        }
    }

    public class FormStatusDescription : ILocalizationHelper
    {
        [CompareField("status_id")]
        public int StatusId { get; set; }

        [CompareField("code")]
        public string Code { get; set; } = string.Empty;

        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("description")]
        public string Description { get; set; } = string.Empty;
    }
}
