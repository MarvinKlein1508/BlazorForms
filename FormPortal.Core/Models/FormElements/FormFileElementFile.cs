using DbController;

namespace FormPortal.Core.Models.FormElements
{
    public class FormFileElementFile : IDbModel
    {
        [CompareField("file_id")]
        public int FileId { get; set; }
        [CompareField("entry_id")]
        public int EntryId { get; set; }
        [CompareField("element_id")]
        public int ElementId { get; set; }
        [CompareField("data")]
        public byte[] Data { get; set; } = Array.Empty<byte>();
        [CompareField("content_type")]
        public string ContentType { get; set; } = string.Empty;
        [CompareField("filename")]
        public string Filename { get; set; } = string.Empty;
        public int Id => FileId;

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "FILE_ID", FileId },
                { "ENTRY_ID", EntryId },
                { "ELEMENT_ID", ElementId },
                { "DATA", Data },
                { "CONTENT_TYPE", ContentType },
                { "FILENAME", Filename },
            };
        }
    }
}
