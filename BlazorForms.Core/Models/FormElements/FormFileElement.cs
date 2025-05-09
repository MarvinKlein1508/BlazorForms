using BlazorForms.Core.Constants;

namespace BlazorForms.Core.Models.FormElements;

public class FormFileElement : FormElement
{
    /// <summary>
    /// Gets the min file size for the uploaded documents in MiB
    /// </summary>
    public int MinSize { get; set; }
    /// <summary>
    /// Gets the max file size for the uploaded documents in MiB
    /// </summary>
    public int MaxSize { get; set; }
    public bool AllowMultipleFiles { get; set; }

    public List<string> AcceptedFileTypes { get; set; } = [];
    public List<FormFileElementFile> Values { get; set; } = [];
    public override ElementType GetElementType() => ElementType.File;

    public override Dictionary<string, object?> GetParameters()
    {
        var parameters = base.GetParameters();
        parameters.Add("MIN_SIZE", MinSize);
        parameters.Add("MAX_SIZE", MaxSize);
        parameters.Add("ALLOW_MULTIPLE_FILES", AllowMultipleFiles);
        return parameters;
    }
    public override string GetDefaultName() => "File";

    public string GetAcceptTypesDefinition(Dictionary<string, string> mimeTypes)
    {
        if (AcceptedFileTypes.Count == 0)
        {
            return string.Empty;
        }

        List<string> allowedMimeTypes = [];

        foreach (var extension in AcceptedFileTypes)
        {
            string blank_extension = extension.Replace(".", string.Empty);
            if (mimeTypes.TryGetValue(blank_extension, out var mimeType) && mimeType is not null)
            {
                allowedMimeTypes.Add(mimeType);
            }
        }

        return string.Join(", ", allowedMimeTypes.Distinct());
    }

    public override void Reset()
    {
        Values.Clear();
    }

    public override void SetValue(FormEntryElement element)
    {
        // No value to be set
    }

    public override object Clone()
    {
        return this.MemberwiseClone();
    }
}
