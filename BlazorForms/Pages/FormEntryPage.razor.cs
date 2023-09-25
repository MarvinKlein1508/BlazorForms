using DbController;
using DbController.MySql;
using BlazorForms.Core.Constants;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;
using BlazorForms.Core.Pdf;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MimeKit;
using System.Globalization;
using BlazorBootstrap;

namespace BlazorForms.Pages
{
    public partial class FormEntryPage
    {
        private Modal _historyModal = default!;
        private Modal _statusModal = default!;
        [Parameter]
        public int FormId { get; set; }
        [Parameter]
        public int EntryId { get; set; }
        [Parameter, SupplyParameterFromQuery]
        public bool Copy { get; set; }
        public FormEntry? Input { get; set; }
        private EditForm? _form;

        private User? _user;
        private bool _isSaving;




        public bool IsAdmin { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsAllowedToApprove { get; set; }
        public bool IsManager { get; set; }
        public bool RequiresApproval { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            _user = await authService.GetUserAsync(dbController);
            if (EntryId > 0)
            {
                var form = await formEntryService.GetAsync(EntryId, dbController);
                if (form is not null)
                {
                    Input = form;
                    FormId = Input?.FormId ?? 0;

                    await CheckPermissionsAsync();
                    bool canSeeEntry = _user is not null && (form.CreationUserId == _user.UserId || form.Form.ManagerUsers.Select(x => x.UserId).Contains(_user.UserId)) || IsAdmin;
                    if (!canSeeEntry)
                    {
                        if (_user is null)
                        {
                            navigationManager.NavigateTo($"/Account/Login?returnUrl=Entry/{EntryId}", true);
                        }
                        else
                        {
                            await jsRuntime.ShowToastAsync(ToastType.error, "Sie verf�gen nicht �ber die ausreichenden Berechtigungen, um diesen Formulareintrag zu bearbeiten.");
                            navigationManager.NavigateTo("/");
                        }
                        return;
                    }

                }
            }

            if ((FormId > 0 && EntryId is 0) || (EntryId > 0 && Copy))
            {
                var form = await formService.GetAsync(FormId, dbController);
                if (form is not null)
                {
                    if (form.IsOnlyAvailableForLoggedInUsers && _user is null)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Um dieses Formular ausf�llen zu k�nnen, m�ssen Sie sich zun�chst einloggen.");
                        navigationManager.NavigateTo($"/Account/Login?returnUrl=Forms/{FormId}", true);
                        return;
                    }

                    if (form.IsOnlyAvailableForLoggedInUsers && form.AllowedUsersForNewEntries.Any() && _user is not null && form.AllowedUsersForNewEntries.FirstOrDefault(x => x.UserId == _user.UserId) is null)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Sie sind nicht berechtigt dieses Formular auszuf�llen.");
                        navigationManager.NavigateTo($"/");
                        return;
                    }

                    if (!form.IsActive)
                    {
                        await jsRuntime.ShowToastAsync(ToastType.error, "Dieses Formular ist nicht aktiviert.");
                        navigationManager.NavigateTo($"/");
                        return;
                    }

                    if (Input is not null && Copy)
                    {
                        // Reset form
                        Input.StatusId = form.DefaultStatusId;
                        Input.IsApproved = false;
                        Input.Name = string.Empty;

                        foreach (var element in Input.Form.GetAllElements())
                        {
                            if (!element.ResetOnCopy)
                            {
                                continue;
                            }

                            element.Reset();
                        }
                    }
                    else
                    {
                        Input = new FormEntry(form)
                        {
                            FormId = FormId,
                            StatusId = form.DefaultStatusId
                        };
                    }
                }
                else
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Formular konnte nicht gefunden werden.");
                    navigationManager.NavigateTo($"/");
                    return;
                }
            }
        }
        private async Task CheckPermissionsAsync()
        {
            if (Input is null || _user is null)
            {
                return;
            }

            IsAdmin = await authService.HasRole(Roles.EDIT_ENTRIES);
            var user = Input.Form.ManagerUsers.FirstOrDefault(x => x.UserId == _user.Id);
            IsManager = user is not null;
            IsAllowedToApprove = user?.CanApprove ?? false;
            var searchStatus = AppdatenService.Get<FormStatus>(Input.StatusId);
            IsCompleted = searchStatus?.IsCompleted ?? false;
            RequiresApproval = searchStatus?.RequiresApproval ?? false;
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
                using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);

                if (Input.EntryId is 0 || Copy)
                {
                    Input.CreationDate = DateTime.Now;
                    Input.CreationUserId = _user?.UserId;
                }

                Input.LastChange = DateTime.Now;
                Input.LastChangeUserId = _user?.UserId;

                await dbController.StartTransactionAsync();
                try
                {
                    if (Input.EntryId is 0 || Copy)
                    {
                        await formEntryService.CreateAsync(Input, dbController);
                    }
                    else
                    {
                        await formEntryService.UpdateAsync(Input, dbController);
                    }

                    await dbController.CommitChangesAsync();


                }
                catch (Exception)
                {
                    await dbController.RollbackChangesAsync();
                    throw;
                }

                // E-Mail support
                if (emailConfig.Value.Enabled && Input.Form.ManagerUsers.Any())
                {
                    MimeMessage email = new MimeMessage();
                    email.From.Add(new MailboxAddress(emailConfig.Value.SenderName, emailConfig.Value.SenderEmail));

                    var status = AppdatenService.Get<FormStatus>(Input.Form.DefaultStatusId) ?? throw new NullReferenceException($"Status cannot be null");

                    foreach (var manager in Input.Form.ManagerUsers)
                    {
                        // When the default status requires someone with approval permissions we'll should only notify people with this permission
                        if (!status.RequiresApproval || (status.RequiresApproval && manager.CanApprove))
                        {
                            if (manager.EmailEnabled && StringExtensions.IsEmail(manager.Email))
                            {
                                email.To.Add(new MailboxAddress(manager.Email, manager.Email));

                            }
                        }
                    }

                    if (email.To.Any())
                    {
                        email.Subject = $"Neuer Formulareintrag f�r {Input.Form.Name}";

                        ReportFormEntry entry = await ReportFormEntry.CreateAsync(Input);
                        var bytes = entry.GetBytes();

                        var body = new TextPart("html")
                        {
                            Text = $"Es wurde ein neuer Eintrag f�r das Formular {Input.Form.Name} abgeschickt. <a href='{navigationManager.BaseUri}Entry/{Input.EntryId}'>Klicken Sie hier</a> um den Formulareintrag zu bearbeiten"
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

                await jsRuntime.ShowToastAsync(ToastType.success, "Formular erfolgreich gespeichert");
                navigationManager.NavigateTo("/");
            }
            _isSaving = false;
        }
        private async Task OpenStatusModalAsync()
        {
                    

            await _statusModal.ShowAsync();
        }
        private async Task OnEntryStatusSavedAsync(FormEntryStatusChange newStatus)
        {
            await _statusModal.HideAsync();
            await OnParametersSetAsync();
        }
        private async Task UploadFileAsync(FormFileElement fileElement, InputFileChangeEventArgs e)
        {
            if (e.FileCount > 10)
            {
                await jsRuntime.ShowToastAsync(ToastType.error, "Es k�nnen maximal 10 Dateien auf einmal hochgeladen werden.");
                return;
            }

            List<string> allowedMimeTypes = new();

            foreach (var extension in fileElement.AcceptedFileTypes)
            {
                string blank_extension = extension.Replace(".", string.Empty).ToLower();
                if (AppdatenService.MimeTypes.TryGetValue(blank_extension, out var mimeType) && mimeType is not null)
                {
                    allowedMimeTypes.Add(mimeType);
                }
            }

            // Some files does not contain a ContentType. So we always aqccept empty ones.
            allowedMimeTypes.Add(string.Empty);

            foreach (var file in e.GetMultipleFiles())
            {
                long size = file.Size;
                long size_in_mib = size / 1024 / 1024;
                string contentType = file.ContentType;
                string filename = file.Name;
                string extension = Path.GetExtension(filename).Replace(".", string.Empty).ToLower();


                // Check file extension in MimeType list
                if (!AppdatenService.MimeTypes.TryGetValue(extension, out var mimeType))
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden. Dateityp wird nicht unterst�tzt.");
                    continue;
                }

                if (!fileElement.AcceptedFileTypes.Contains(extension) || !allowedMimeTypes.Contains(contentType))
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden, ung�ltiges Dateiformat.");
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
                        await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden. (zu gro�)");
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
                    await jsRuntime.ShowToastAsync(ToastType.error, "Datei konnte nicht hochgeladen werden, dieses Feld unterst�tzt nur den Upload einer Datei.");
                }

            }
        }
        private string GetStatus()
        {
            if (Input is null)
            {
                return string.Empty;
            }
            var status = AppdatenService.Get<FormStatus>(Input.StatusId);

            if (status is null)
            {
                return string.Empty;
            }

            var description = status.GetLocalization(CultureInfo.CurrentCulture);

            return description?.Name ?? string.Empty;
        }

        private async Task ShowHistoryModalAsync()
        {
            await _historyModal.ShowAsync();
        }
    }
}