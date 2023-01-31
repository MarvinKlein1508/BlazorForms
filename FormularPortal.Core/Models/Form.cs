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
    }
}
