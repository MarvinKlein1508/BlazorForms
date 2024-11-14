using BlazorForms.Core;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using DbController;
using DbController.MySql;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace BlazorForms.Components.Pages.Admin
{
    public partial class UserManagement
    {
        private User? _loggedInUser;
        public UserFilter Filter { get; set; } = new()
        {
            Limit = Storage.PageLimit
        };
        public Permission? SelectedPermission { get; set; }
        [Parameter, SupplyParameterFromQuery]
        public int Page { get; set; } = 1;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / (double)Filter.Limit);
        protected override async Task OnParametersSetAsync()
        {
            if (Page <= 0)
            {
                Page = 1;
            }

            _loggedInUser = await authService.GetUserAsync();
            await LoadAsync();
        }
        protected override async Task SaveAsync()
        {
            if (Input is null)
            {
                return;
            }

            PasswordHasher<User> hasher = new();
            string passwordHashed = hasher.HashPassword(Input, Input.Password + Input.Salt);
            Input.Password = passwordHashed;

            await base.SaveAsync();
            await LoadAsync();
        }
        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                Filter.PageNumber = 1;
                navigationManager.NavigateTo("/Admin/Users");
            }

            Filter.PageNumber = navigateToPage1 ? 1 : Page;
            using IDbController dbController = new MySqlController();
            TotalItems = await Service.GetTotalAsync(Filter, dbController);
            Data = await Service.GetAsync(Filter, dbController);
        }

        private Task OnPageChangedAsync(int pageNumber)
        {
            navigationManager.NavigateTo($"/Admin/Users?page={pageNumber}");
            return Task.CompletedTask;
        }

        protected override Task NewAsync()
        {
            Input = new User
            {
                Origin = "local",
                Salt = StringExtensions.RandomString(10)
            };

            return Task.CompletedTask;
        }
        private Task AddPermissionAsync()
        {
            if (Input is not null)
            {
                if (SelectedPermission is not null)
                {
                    Input.Permissions.Add(SelectedPermission);
                }

                SelectedPermission = null;
            }

            return Task.CompletedTask;
        }
        private Task PermissionSelectionChangedAsync(ChangeEventArgs e)
        {
            int permissionId = Convert.ToInt32(e.Value);
            SelectedPermission = Storage.Get<Permission>().FirstOrDefault(x => x.PermissionId == permissionId);
            return Task.CompletedTask;
        }

        private async Task ShowDeleteModalAsync(User input)
        {
            bool isDeleted = await base.ShowDeleteModalAsync(input, localizer["MODAL_DELETE_TITLE"], String.Format(localizer["MODAL_DELETE_TEXT"], input.DisplayName), localizer["MODAL_DELETE_SUCCESS"]);
            if (isDeleted)
            {
                await LoadAsync();
            }
        }
    }
}