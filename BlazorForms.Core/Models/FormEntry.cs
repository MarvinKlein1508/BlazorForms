using BlazorForms.Core.Enums;
using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models.FormElements;

namespace BlazorForms.Core.Models;

public class FormEntry : IDbModel<int?>
{
    public int EntryId { get; set; }
    public int FormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public int? CreationUserId { get; set; }
    public DateTime LastChange { get; set; } = DateTime.Now;
    public int? LastChangeUserId { get; set; }
    public int StatusId { get; set; }
    public bool IsApproved { get; set; }
    public Priority Priority { get; set; } = Priority.Normal;
    public Form Form { get; set; }
    public int? GetIdentifier()
    {
        return EntryId > 0 ? EntryId : null;
    }
    public Dictionary<string, object?> GetParameters()
    {
        return new Dictionary<string, object?>
        {
            { "ENTRY_ID",  EntryId },
            { "FORM_ID",  FormId },
            { "NAME", Name },
            { "CREATION_DATE",  CreationDate },
            { "CREATION_USER_ID",  CreationUserId is 0 ? null : CreationUserId },
            { "LAST_CHANGE",  LastChange },
            { "LAST_CHANGE_USER_ID",  LastChangeUserId is 0 ? null : LastChangeUserId },
            { "STATUS_ID", StatusId },
            { "APPROVED", IsApproved },
            { "PRIORITY", (int)Priority }
        };
    }

    public FormEntry()
    {
        Form = new();
    }

    public FormEntry(Form form)
    {
        form.EntryMode = true;
        Form = form;

        foreach (var element in form.GetElements())
        {
            if (element is FormTableElement tableElement)
            {
                tableElement.NewRow();
            }
            else if (element is FormDateElement dateElement)
            {
                if (dateElement.SetDefaultValueToCurrentDate)
                {
                    dateElement.Value = DateTime.Now;
                }
            }
        }
    }
}
