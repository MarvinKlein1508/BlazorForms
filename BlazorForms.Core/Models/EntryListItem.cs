using BlazorForms.Core.Enums;

namespace BlazorForms.Core.Models;
public class EntryListItem
{
    public int EntryId { get; set; }
    public int FormId { get; set; }
    public int StatusId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public int? CreationUserId { get; set; }
    public DateTime LastChange { get; set; } = DateTime.Now;
    public int? LastChangeUserId { get; set; }
    public string FormName { get; set; } = string.Empty;
    public string UsernameCreator { get; set; } = string.Empty;
    public string UsernameLastChange { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public List<int> ManagerIds { get; set; } = [];

}
