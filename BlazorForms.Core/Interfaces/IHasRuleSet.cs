using BlazorForms.Core.Constants;
using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Interfaces
{
    public interface IHasRuleSet
    {
        List<Rule> Rules { get; set; }
        Form? Form { get; set; }
        RuleType RuleType { get; set; }

        IEnumerable<FormElement> GetRuleElements();
    }
}
