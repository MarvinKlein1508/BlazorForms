using BlazorForms.Core.Constants;
using BlazorForms.Core.Models;

namespace BlazorForms.Core.Interfaces
{
    public interface IHasRuleSet
    {
        List<Rule> Rules { get; set; }
        Form? Form { get; set; }
        RuleType RuleType { get; set; }
    }
}
