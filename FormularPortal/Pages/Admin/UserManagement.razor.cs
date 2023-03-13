using Blazor.Pagination;
using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using FormPortal.Core.Models;
using FormPortal.Core.Services;
using FormPortal.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace FormPortal.Pages.Admin
{
    public partial class UserManagement : IHasPagination
    {
        private int _page = 1;
        private User? _loggedInUser;
        public UserFilter Filter { get; set; } = new();
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
            using IDbController dbController = DbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            TotalItems = await Service.GetTotalAsync(Filter, dbController);
            Data = await Service.GetAsync(Filter, dbController);
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