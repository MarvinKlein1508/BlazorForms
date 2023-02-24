using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using FormularPortal;
using FormularPortal.Components;
using Plk.Blazor.DragDrop;
using FormPortal.Core.Services;
using FormularPortal.Components.Modals;
using DatabaseControllerProvider;
using Blazor.Pagination;
using Blazor.BootstrapTabs;
using FormularPortal.Core;
using BlazorTooltips;
using vNext.BlazorComponents.FluentValidation;
using FormPortal.Core.Constants;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;
using FormPortal.Core.Models.FormElements;
using BlazorContextMenu;
using CKEditor;

namespace FormularPortal.Components
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
    }
}