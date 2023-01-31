using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Filters
{
    public class FormFilter : PageFilterBase
    {
        public bool ShowOnlyActiveForms { get; set; }
        public bool ShowOnlyFormsWhichRequireLogin { get; set; }
    }
}
