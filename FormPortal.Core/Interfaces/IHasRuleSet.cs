using FormPortal.Core.Constants;
using FormPortal.Core.Models;

namespace FormPortal.Core.Interfaces
{
    public interface IHasRuleSet
    {
        List<Rule> Rules { get; set; }
        Form? Form { get; set; }
        RuleType RuleType { get; set; }
    }
}
