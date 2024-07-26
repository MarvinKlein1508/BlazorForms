using BlazorForms.Core.Models;
using DbController;
using Microsoft.AspNetCore.Identity;
using Spectre.Console;

namespace BlazorForms
{
    public static class DbInstaller
    {
        public static string HashPassword(User user)
        {
            PasswordHasher<User> hasher = new();
            string passwordHashed = hasher.HashPassword(user, user.Password + user.Salt);

            return passwordHashed;
        } 
    }
}
