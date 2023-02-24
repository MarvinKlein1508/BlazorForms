using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Services;
using FormPortal.Core.Validators;
using FormPortal.Core.Validators.Admin;
using FormularPortal.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FormularPortal.Pages
{
    public partial class FormEntryPage
    {
        [Parameter]
        public int FormId { get; set; }
        public FormEntry? Input { get; set; }
        private EditForm? _form;
        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            var form = await formService.GetAsync(FormId, dbController);


            if (form is not null)
            {
                Input = new FormEntry(form);
            }
        }

        private async Task SubmitAsync()
        {
            if(_form is null || _form.EditContext is null)
            {
                return;
            }

            _form.EditContext.Validate();
        }

        private async Task UploadFileAsync(FormFileElement fileElement, InputFileChangeEventArgs e)
        {

            foreach (var file in e.GetMultipleFiles())
            {
                long size = file.Size;
                long size_in_mib = size / 1024 / 1024;
                string contentType = file.ContentType;
                string filename = file.Name;
                string[] allowedContentType = fileElement.AcceptFileTypes.Split(',', StringSplitOptions.TrimEntries);
                if(!allowedContentType.Contains(contentType))
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden, ungültiges Dateiformat.");
                    continue;
                }

                if(fileElement.MinSize > 0)
                {
                    if(size_in_mib < fileElement.MinSize)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden (zu klein).");
                        continue;
                    }
                }

                if(fileElement.MaxSize > 0)
                {
                    long sizeInMiB = size / 1024 / 1024;
                    if(sizeInMiB > fileElement.MaxSize)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden. (zu groß)");
                        continue;
                    }
                }

                if (fileElement.AllowMultipleFiles || !fileElement.Values.Any())
                {
                    await using MemoryStream fs = new();
                    await file.OpenReadStream(file.Size).CopyToAsync(fs);
                    fileElement.Values.Add(new FormFileElementFile
                    {
                        ContentType = contentType,
                        Data = fs.ToArray(),
                        Filename = filename
                    });
                }
                else
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden, dieses Feld unterstützt nur den Upload einer Datei.");
                }

            }




        }
    }
}