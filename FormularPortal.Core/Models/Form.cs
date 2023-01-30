using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Models
{
    public class Form
    {
        public int FormId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsOnlyAvailableForLoggedInUsers { get; set; }
        public bool IsActive { get; set; }
        public List<FormRow> Rows { get; set; } = new();
    }
}
