using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FormPortal.Pages
{
    public partial class FormEntryPage
    {
        [Parameter]
        public int FormId { get; set; }
        [Parameter]
        public int EntryId { get; set; }
        public FormEntry? Input { get; set; }
        private EditForm? _form;

        private User? _user;
        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            _user = await authService.GetUserAsync(dbController);

            if (EntryId > 0)
            {
                Input = await formEntryService.GetAsync(EntryId, dbController);
                FormId = Input?.FormId ?? 0;
            }
            else if (FormId > 0)
            {
                var form = await formService.GetAsync(FormId, dbController);
                if (form is not null)
                {
                    if (form.IsOnlyAvailableForLoggedInUsers && _user is null)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Um dieses Formular ausfüllen zu können, müssen Sie sich zunächst einloggen.");
                        navigationManager.NavigateTo($"/");
                        return;
                    }

                    if (!form.IsActive)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Dieses Formular ist nicht aktiviert.");
                        navigationManager.NavigateTo($"/");
                        return;
                    }

                    Input = new FormEntry(form)
                    {
                        FormId = FormId
                    };
                }
                else
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Formular konnte nicht gefunden werden.");
                    navigationManager.NavigateTo($"/");
                    return;
                }
            }
        }

        private async Task SubmitAsync()
        {
            if (_form is null || _form.EditContext is null || Input is null)
            {
                return;
            }

            if (_form.EditContext.Validate())
            {
                using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);

                if (Input.EntryId is 0)
                {
                    Input.CreationDate = DateTime.Now;
                    Input.CreationUserId = _user?.UserId;
                }

                Input.LastChange = DateTime.Now;
                Input.LastChangeUserId = _user?.UserId;

                await dbController.StartTransactionAsync();
                try
                {
                    if (Input.EntryId is 0)
                    {
                        await formEntryService.CreateAsync(Input, dbController);
                    }
                    else
                    {
                        await formEntryService.UpdateAsync(Input, dbController);
                    }

                    await dbController.CommitChangesAsync();
                    await jsRuntime.ShowToastAsync(ToastType.success, "Formular erfolgreich gespeichert");

                }
                catch (Exception)
                {
                    await dbController.RollbackChangesAsync();
                    throw;
                }

                navigationManager.NavigateTo("/");
            }
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
                if (!allowedContentType.Contains(contentType))
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden, ungültiges Dateiformat.");
                    continue;
                }

                if (fileElement.MinSize > 0)
                {
                    if (size_in_mib < fileElement.MinSize)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden (zu klein).");
                        continue;
                    }
                }

                if (fileElement.MaxSize > 0)
                {
                    long sizeInMiB = size / 1024 / 1024;
                    if (sizeInMiB > fileElement.MaxSize)
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