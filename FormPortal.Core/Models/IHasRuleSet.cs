using FormPortal.Core.Constants;

namespace FormPortal.Core.Models
{
    public interface IHasRuleSet
    {
        List<Rule> Rules { get; set; }
        Form? Form { get; set; }
        RuleType RuleType { get; set; }
    }
}
