using BlazorForms.Application.Auth;
using BlazorForms.Application.Domain;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace BlazorForms.Web.Components.Pages.Admin;
public partial class UserCreate
{
    protected override string GetEntityRedirectUrl() => $"/Admin/Users/Edit?Id={Input!.GetIdentifier()}";
    public bool ActiveDirectoryMode => Input?.Origin is "ad";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Input is not null)
        {
            Input.Origin = "local";
        }
    }
    protected override Task BeforeSaveAsync(IDbConnection connection, IDbTransaction transaction)
    {
        if (Input!.Origin != "ad")
        {
            PasswordHasher<User> hasher = new();
            Input.Salt = SaltGenerator.GenerateSaltBase64();
            string passwordHashed = hasher.HashPassword(Input, Input.Password + Input.Salt);
            Input.Password = passwordHashed;
        }

        return Task.CompletedTask;
    }
}