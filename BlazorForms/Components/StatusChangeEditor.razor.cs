using DbController;
using DbController.MySql;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Models;
using BlazorForms.Core.Pdf;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MimeKit;
using Microsoft.AspNetCore.Mvc;

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

        private List<User> _availableForNotification = new();

        protected override async Task OnInitializedAsync()
        {
            ArgumentNullException.ThrowIfNull(User, nameof(User));
            ArgumentNullException.ThrowIfNull(Entry, nameof(Entry));

            _availableForNotification = await SetupNotifiersAsync(Entry, User);

            Input = new()
            {
                UserId = User.Id,
                EntryId = Entry.Id
            };

            foreach (var user in _availableForNotification)
            {
                Input.Notifiers.Add(new FormEntryHistoryNotify
                {
                    UserId = user.Id
                });
            }
        }
  

        private async Task<List<User>> SetupNotifiersAsync(FormEntry entry, User currentUser)
        {
            List<User> result = new();
            List<int> userIds = new();

            // Generate list of available notifiers
            foreach (var user in entry.Form.ManagerUsers)
            {
                // We don't want to notify ourself
                if (user.Id == currentUser.Id)
                {
                    continue;
                }

                result.Add(user);
                userIds.Add(user.Id);
            }

            if (entry.CreationUserId is int creationUserId && currentUser.Id != creationUserId && !userIds.Contains(creationUserId))
            {
                using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
                User? creationUser = await userService.GetAsync((int)entry.CreationUserId, dbController);
                if (creationUser is not null)
                {
                    result.Add(creationUser);
                    userIds.Add(creationUserId);
                }
            }


            return result;
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
                if (emailSettings.Value.Enabled && Input.Notifiers.Any(x => x.Notify))
                {
                    List<string> email_addresses = new();
                    
                    foreach (var notify in Input.Notifiers.Where(x => x.Notify))
                    {
                        var user = _availableForNotification.First(x => x.Id == notify.UserId);
                        email_addresses.Add(user.Email);
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