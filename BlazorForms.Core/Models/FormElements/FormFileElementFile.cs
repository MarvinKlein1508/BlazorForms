namespace BlazorForms.Core.Models.FormElements;

public class FormFileElementFile : IDbModel<int?>
{
    public int FileId { get; set; }
    public int EntryId { get; set; }
    public int ElementId { get; set; }
    public byte[] Data { get; set; } = [];
    public string ContentType { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
    public int? GetIdentifier()
    {
        return FileId > 0 ? FileId : null;
    }

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
