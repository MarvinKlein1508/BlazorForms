using FormPortal.Core.Models.FormElements;
using Microsoft.AspNetCore.Components;

namespace FormPortal.Components
{
    public partial class FileListing
    {
        [Parameter, EditorRequired]
        public List<FormFileElementFile> Files { get; set; } = new();


        private bool IsImage(FormFileElementFile file) => file.ContentType.StartsWith("image/");

        private string GetBase64Image(FormFileElementFile file)
        {
            string base64Image = Convert.ToBase64String(file.Data);

            return $"data:{file.ContentType};base64, {base64Image}";
        }

        private Task DeleteAsync(FormFileElementFile file)
        {
            Files.Remove(file);
            return Task.CompletedTask;
        }

        private async Task DownloadAsync(FormFileElementFile file)
        {
            await downloadService.DownloadFile(file.Filename, file.Data, file.ContentType);
        }
        private string GetFileIcon(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();

            return ext switch
            {
                ".doc" or ".docx" => "fa-file-word",
                ".ppt" or ".pptx" => "fa-file-powerpoint",
                ".pdf" => "fa-file-pdf",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "fa-file-image",
                ".xlsx" or ".xls" => "fa-file-excel",
                ".csv" => "fa-file-csv",
                ".zip" or ".7z" or ".rar" or ".tar" or ".gz" or ".tar.gz" => "fa-file-archive",
                ".mp3" or ".wav" => "fa-file-audio",
                ".mp4" => "fa-file-video",
                _ => "fa-file",
            };
        }

        public static string ToHumanReadableFileSize(byte[] data)
        {
            long bytes = data.LongLength;
            string[] sizes = { "B", "KiB", "MiB", "GiB", "TiB" };
            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }
            return String.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
    }
}