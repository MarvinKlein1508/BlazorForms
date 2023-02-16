namespace FormPortal.Core.Models
{
    public interface IHasRuleSet
    {
        List<Rule> Rules { get; set; }
        Form? Form { get; set; }
    }
}
