using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Models
{
    public class FormStatus : ILocalizedDbModel<FormStatusDescription>
    {
        [CompareField("status_id")]
        public int Id { get; set; }
        [CompareField("requires_approval")]
        public bool RequiresApproval { get; set; }
        [CompareField("is_completed")]
        public bool IsCompleted { get; set; }
        public List<FormStatusDescription> Description { get; set; } = new();

        public IEnumerable<Dictionary<string, object?>> GetLocalizedParameters()
        {
            foreach (var item in Description)
            {
                yield return new Dictionary<string, object?>
                {
                    { "STATUS_ID", Id },
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
                { "STATUS_ID", Id },
                { "REQUIRES_APPROVAL", RequiresApproval },
                { "IS_COMPLETED", IsCompleted }
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
