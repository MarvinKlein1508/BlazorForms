using DbController;
using DbController.MySql;
using BlazorForms.Core.Extensions;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace BlazorForms.Pages.Admin
{
    public partial class UserManagement
    {
        private int _page = 1;
        private User? _loggedInUser;
        public UserFilter Filter { get; set; } = new()
        {
            Limit = AppdatenService.PageLimit
        };
        public Permission? SelectedPermission { get; set; }
        public int Page { get => _page; set => _page = value < 1 ? 1 : value; }
        public int TotalItems { get; set; }
        protected override async Task OnParametersSetAsync()
        {
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
            Filter.PageNumber = navigateToPage1 ? 1 : Page;
            using IDbController dbController = new MySqlController(AppdatenService.ConnectionString);
            TotalItems = await Service.GetTotalAsync(Filter, dbController);
            Data = await Service.GetAsync(Filter, dbController);
        }

        protected override async Task DeleteAsync()
        {
            await base.DeleteAsync();
            await LoadAsync();
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
            SelectedPermission = AppdatenService.Permissions.FirstOrDefault(x => x.PermissionId == permissionId);
            return Task.CompletedTask;
        }
    }
}