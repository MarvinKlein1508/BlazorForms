using DatabaseControllerProvider;

namespace FormPortal.Core.Models.FormElements
{
    public class FormFileElement : FormElement
    {
        [CompareField("accept_file_types")]
        public string AcceptFileTypes { get; set; } = string.Empty;
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
        public List<FormFileElementFile> Values { get; set; } = new();
        public override ElementType GetElementType() => ElementType.File;

        public override Dictionary<string, object?> GetParameters()
        {
            var parameters = base.GetParameters();
            parameters.Add("ACCEPT_FILE_TYPES", AcceptFileTypes);
            parameters.Add("MIN_SIZE", MinSize);
            parameters.Add("MAX_SIZE", MaxSize);
            parameters.Add("ALLOW_MULTIPLE_FILES", AllowMultipleFiles);
            return parameters;
        }
        public override string GetDefaultName() => "File";
    }
}
