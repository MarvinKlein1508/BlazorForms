using DbController;

namespace BlazorForms.Core.Models
{
    public sealed class FormManagerMapping
    {
        [CompareField("form_id")]
        public int FormId { get; set; }
        [CompareField("user_id")]
        public int UserId { get; set; }
    }
}
