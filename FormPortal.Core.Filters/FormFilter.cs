using FormPortal.Core.Filters.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormPortal.Core.Filters
{
    public class FormFilter : PageFilterBase
    {
        public bool ShowOnlyActiveForms { get; set; }
        public bool ShowOnlyFormsWhichRequireLogin { get; set; }
    }
}
