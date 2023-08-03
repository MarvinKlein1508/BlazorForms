using DbController;
using DbController.MySql;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Models;
using BlazorForms.Core.Pdf;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MimeKit;

namespace BlazorForms.Components
{
    public partial class StatusChangeEditor
    {
        [Parameter, EditorRequired]
        public FormEntry? Entry { get; set; }
        [Parameter, EditorRequired]
        public User? User { get; set; }
        [Parameter, EditorRequired]
        public EventCallback<FormEntryStatusChange> OnSaved { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }

        public FormEntryStatusChange? Input { get; set; }
        private EditForm? _form;

        protected override Task OnParametersSetAsync()
        {
            ArgumentNullException.ThrowIfNull(User, nameof(User));
            ArgumentNullException.ThrowIfNull(Entry, nameof(Entry));

            Input = new()
            {
                UserId = User.Id,
                EntryId = Entry.Id
            };

            return base.OnParametersSetAsync();
        }

        private async Task SaveAsync()
        {
            if (Input is null || _form is null || _form.EditContext is null || Entry is null)
            {
                return;
            }

            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            await dbController.StartTransactionAsync();
            Input.DateAdded = DateTime.Now;

            if (_form.EditContext.Validate())
            {
                try
                {
                    if (Input.Id is 0)
                    {
                        await statusChangeService.CreateAsync(Input, dbController);
                    }
                    else
                    {
                        await statusChangeService.UpdateAsync(Input, dbController);
                    }

                    // Update the status of the entry
                    // Check if old status needed approve
                    var status = AppdatenService.Get<FormStatus>(Entry.StatusId);
                    if (status is not null && status.RequiresApproval)
                    {
                        // We only approve once, even when the second status requires another approval
                        await statusChangeService.ApproveAsync(Entry.Id, dbController);
                        Entry.IsApproved = true;

                    }
                    Entry.StatusId = Input.StatusId;
                    await dbController.CommitChangesAsync();
                }
                catch (Exception)
                {
                    await dbController.RollbackChangesAsync();
                    throw;
                }


                // Send E-Mails
                if (emailSettings.Value.Enabled && (Input.NotifyCreator || Input.NotifyManagers || Input.NotifyApprovers))
                {
                    List<string> email_addresses = new();
                    if (Input.NotifyCreator)
                    {
                        if (Entry.CreationUserId != null)
                        {
                            User? user = await userService.GetAsync((int)Entry.CreationUserId, dbController);

                            if (user is not null)
                            {
                                email_addresses.Add(user.Email);    
                            }
                        }
                    }

                    if (Input.NotifyManagers)
                    {
                        // Don't send emails to yourself
                        email_addresses.AddRange(Entry.Form.ManagerUsers.Where(x => x.EmailEnabled && (User is null || x.UserId != User.Id)).Select(x => x.Email));
                    }

                    if (Input.NotifyApprovers)
                    {
                        // Don't send emails to yourself
                        email_addresses.AddRange(Entry.Form.ManagerUsers.Where(x => x.EmailEnabled && x.CanApprove && (User is null || x.UserId != User.Id)).Select(x => x.Email));
                    }

                    if (email_addresses.Any())
                    {
                        await Input.SendMailForEntryStatusChangeAsync(email_addresses, Entry, navigationManager.BaseUri, emailSettings.Value);
                    }
                }

                await JSRuntime.ShowToastAsync(ToastType.success, AppLocalizer["SAVE_MESSAGE"]);
                await OnSaved.InvokeAsync(Input);
            }
        }
    }
}