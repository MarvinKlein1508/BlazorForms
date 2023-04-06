using DatabaseControllerProvider;
using FormPortal.Core;
using FormPortal.Core.Constants;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Pdf;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MimeKit;

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
        private bool _isSaving;

        private async Task<bool> CanEditEntryAsync(FormEntry? entry)
        {
            if (entry is null)
            {
                return false;
            }

            bool hasEditRole = await authService.HasRole(Roles.EDIT_ENTRIES);

            if (hasEditRole)
            {
                return true;
            }

            if (_user is null)
            {
                return false;
            }

            return entry.CreationUserId == _user.UserId || entry.Form.ManagerUsers.Select(x => x.UserId).Contains(_user.UserId);
        }
        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            _user = await authService.GetUserAsync(dbController);

            if (EntryId > 0)
            {
                var form = await formEntryService.GetAsync(EntryId, dbController);
                if (form is not null)
                {
                    Input = form;
                    FormId = Input?.FormId ?? 0;

                    bool canEdit = await CanEditEntryAsync(Input);
                    if (!canEdit)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Sie verfügen nicht über die ausreichenden Berechtigungen, um diesen Formulareintrag zu bearbeiten.");
                        navigationManager.NavigateTo("/");
                        return;
                    }
                }
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

                    if (form.IsOnlyAvailableForLoggedInUsers && form.AllowedUsersForNewEntries.Any() && _user is not null && form.AllowedUsersForNewEntries.FirstOrDefault(x => x.UserId == _user.UserId) is null)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Sie sind nicht berechtigt dieses Formular auszufüllen.");
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

            _isSaving = true;
            if (_form.EditContext.Validate())
            {
                using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);

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

                        if (emailConfig.Value.Enabled && Input.Form.ManagerUsers.Any())
                        {
                            MimeMessage email = new MimeMessage();
                            email.From.Add(new MailboxAddress(emailConfig.Value.SenderName, emailConfig.Value.SenderEmail));
                            foreach (var manager in Input.Form.ManagerUsers)
                            {
                                if (manager.EmailEnabled && StringExtensions.IsEmail(manager.Email))
                                {
                                    email.To.Add(new MailboxAddress(manager.Email, manager.Email));

                                }
                            }

                            if (email.To.Any())
                            {
                                email.Subject = $"Neuer Formulareintrag für {Input.Form.Name}";

                                ReportFormEntry entry = await ReportFormEntry.CreateAsync(Input);
                                var bytes = entry.GetBytes();

                                var body = new TextPart("html")
                                {
                                    Text = $"Es wurde ein neuer Eintrag für das Formular {Input.Form.Name} abgeschickt. <a href='{navigationManager.BaseUri}Entry/{Input.EntryId}'>Klicken Sie hier</a> um den Formulareintrag zu bearbeiten"
                                };

                                string filename = Input.Name;

                                if (string.IsNullOrWhiteSpace(filename))
                                {
                                    filename = $"{Input.Form.Name}_{Input.EntryId}";
                                }

                                using MemoryStream memoryStream = new MemoryStream(bytes);
                                // create an image attachment for the file located at path
                                var attachment = new MimePart("application", "pdf")
                                {
                                    Content = new MimeContent(memoryStream),
                                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                                    ContentTransferEncoding = ContentEncoding.Base64,
                                    FileName = $"{filename}.pdf"
                                };

                                // now create the multipart/mixed container to hold the message text and the
                                // image attachment
                                var multipart = new Multipart("mixed")
                                {
                                    body,
                                    attachment
                                };

                                // now set the multipart/mixed as the message body
                                email.Body = multipart;

                                try
                                {
                                    EmailExtensions.SendMail(email, emailConfig.Value);

                                }
                                catch (Exception ex)
                                {
                                    await jsRuntime.ShowToastAsync(ToastType.error, $"E-Mail konnte nicht gesendet werden. Fehler: {ex}", 0);
                                }
                            }
                        }
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
            _isSaving = false;
        }

        private async Task UploadFileAsync(FormFileElement fileElement, InputFileChangeEventArgs e)
        {
            if (e.FileCount > 10)
            {
                await jsRuntime.ShowToastAsync(ToastType.error, "Es können maximal 10 Dateien auf einmal hochgeladen werden.");
                return;
            }

            List<string> allowedMimeTypes = new();

            foreach (var extension in fileElement.AcceptedFileTypes)
            {
                string blank_extension = extension.Replace(".", string.Empty);
                if (AppdatenService.MimeTypes.TryGetValue(blank_extension, out var mimeType) && mimeType is not null)
                {
                    allowedMimeTypes.Add(mimeType);
                }
            }

            foreach (var file in e.GetMultipleFiles())
            {
                long size = file.Size;
                long size_in_mib = size / 1024 / 1024;
                string contentType = file.ContentType;
                string filename = file.Name;
                string extension = Path.GetExtension(filename).Replace(".", string.Empty);


                // Check file extension in MimeType list
                if (!AppdatenService.MimeTypes.TryGetValue(extension, out var mimeType))
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden. Dateityp wird nicht unterstützt.");
                    continue;
                }

                if (!fileElement.AcceptedFileTypes.Contains(extension) || !allowedMimeTypes.Contains(contentType))
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