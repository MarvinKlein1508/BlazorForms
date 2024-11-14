using DbController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorForms.Core.Models
{
    public class Language : IDbModelWithName
    {
        [CompareField("language_id")]
        public int UserFilterId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("code")]
        public string Code { get; set; } = string.Empty;
        [CompareField("sort_order")]
        public int SortOrder { get; set; }
        [CompareField("status")]
        public bool Status { get; set; }

        public Dictionary<string, object?> GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
