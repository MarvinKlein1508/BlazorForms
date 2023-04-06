using DatabaseControllerProvider;
using FormPortal.Core.Constants;

namespace FormPortal.Core.Models.FormElements
{
    public class FormFileElement : FormElement
    {
        /// <summary>
        /// Gets the min file size for the uploaded documents in MiB
        /// </summary>
        [CompareField("min_size")]
        public int MinSize { get; set; }
        /// <summary>
        /// Gets the max file size for the uploaded documents in MiB
        /// </summary>
        [CompareField("max_size")]
        public int MaxSize { get; set; }
        [CompareField("allow_multiple_files")]
        public bool AllowMultipleFiles { get; set; }

        public List<string> AcceptedFileTypes { get; set; } = new();
        public List<FormFileElementFile> Values { get; set; } = new();
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
            if (!AcceptedFileTypes.Any())
            {
                return string.Empty;
            }

            List<string> allowedMimeTypes = new();

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


    }
}
