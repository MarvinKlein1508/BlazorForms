using BlazorForms.Core.Models.FormElements;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components
{
    public partial class FileListing
    {
        [Parameter, EditorRequired]
        public List<FormFileElementFile> Files { get; set; } = [];


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
                ".doc" or ".docx" => "bi bi-file-earmark-word-fill",
                ".ppt" or ".pptx" => "bi bi-file-earmark-ppt-fill",
                ".pdf" => "bi bi-file-earmark-pdf-fill",
                ".jpg" or ".jpeg" or ".png" or ".gif" => "bi bi-file-earmark-image-fill",
                ".xlsx" or ".xls" => "bi bi-file-earmark-excel-fill",
                ".csv" => "bi bi-filetype-csv",
                ".zip" or ".7z" or ".rar" or ".tar" or ".gz" or ".tar.gz" => "bi bi-file-earmark-zip-fill",
                ".mp3" or ".wav" => "bi bi-file-earmark-music-fill",
                ".mp4" => "bi bi-file-earmark-play-fill",
                _ => "bi bi-file-earmark-fill",
            };
        }

        public static string ToHumanReadableFileSize(byte[] data)
        {
            long bytes = data.LongLength;
            string[] sizes = ["B", "KiB", "MiB", "GiB", "TiB"];
            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes /= 1024;
            }
            return string.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
    }
}