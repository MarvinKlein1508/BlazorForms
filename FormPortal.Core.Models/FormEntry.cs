using DatabaseControllerProvider;
using FormPortal.Core.Models.FormElements;

namespace FormPortal.Core.Models
{
    public class FormEntry
    {
        [CompareField("entry_id")]
        public int EntryId { get; set; }
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("creation_date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;
        [CompareField("creation_user_id")]
        public int? CreationUserId { get; set; }
        [CompareField("last_change")]
        public DateTime LastChange { get; set; } = DateTime.Now;
        [CompareField("last_change_user_id")]
        public int LastChangeUserId { get; set; }
        /// <summary>
        /// Gets all values submitted by the user.
        /// </summary>
        public List<FormEntryElement> EntryElements { get; set; } = new();
        /// <summary>
        /// Gets all values for table elements which has been submitted by the user.
        /// </summary>
        public List<FormEntryTableElement> EntryTableElements { get; set; } = new();
        public Form Form { get; set; }
        /// <summary>
        /// Removes all elements from the <see cref="Form"/> instance which are not part of the <see cref="EntryElements"/>
        /// </summary>
        public void CleanElements()
        {
            throw new NotImplementedException();
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
                if (element is FormTableElement formTableElement)
                {
                    formTableElement.NewRow();
                }
            }
        }






    }
}
