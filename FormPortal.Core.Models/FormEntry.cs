using DatabaseControllerProvider;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    public class FormEntry : IDbModel
    {
        [CompareField("entry_id")]
        public int EntryId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("creation_date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [CompareField("creation_user_id")]
        public int? CreationUserId { get; set; }
        [CompareField("last_change")]
        public DateTime LastChange { get; set; } = DateTime.Now;
        [CompareField("last_change_user_id")]
        public int? LastChangeUserId { get; set; }
        public Form Form { get; set; }
        public int Id => EntryId;
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
                { "LAST_CHANGE_USER_ID",  LastChangeUserId is 0 ? null : LastChangeUserId }
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
}
