namespace BlazorForms.Core.Infrastructure;

public class GraphOptions
{
    public const string SectionName = "GraphOptions";
    public bool Enabled { get; set; }
    public bool Debug { get; set; }
    public required string SenderEmail { get; set; }
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
