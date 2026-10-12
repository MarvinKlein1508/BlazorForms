using BlazorForms.Infrastructure.Database;
using BlazorForms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Provides user authentication services including Active Directory (LDAP)
/// authentication and local username/password verification.
/// </summary>
public class LoginService
{
    private readonly IDbConnectionFactory _dbFactory;
    private readonly UserRepository _userRepository;
    private readonly LdapProvider _ldapProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginService"/> class.
    /// </summary>
    /// <param name="dbFactory">Factory for creating database connections.</param>
    /// <param name="userRepository">Repository for user persistence operations.</param>
    /// <param name="ldapProvider">Provider used to authenticate against LDAP/Active Directory.</param>
    public LoginService(IDbConnectionFactory dbFactory, UserRepository userRepository, LdapProvider ldapProvider)
    {
        _dbFactory = dbFactory;
        _userRepository = userRepository;
        _ldapProvider = ldapProvider;
    }

    /// <summary>
    /// Attempts to authenticate a user against Active Directory using the provided
    /// username and password. If the user does not exist in the local database a
    /// new user record will be created. If the user exists their attributes will
    /// be updated from Active Directory.
    /// </summary>
    /// <param name="username">The username to authenticate.</param>
    /// <param name="password">The password for the username.</param>
    /// <returns>The authenticated <see cref="User"/> on success; otherwise <c>null</c>.</returns>
    public async Task<User?> LdapLoginAsync(string username, string password)
    {
        User? user;

        LdapResult? ldapResult = _ldapProvider.Authenticate(username, password);

        if (ldapResult is null || ldapResult.Guid is null)
        {
            return null;
        }

        using var connection = await _dbFactory.CreateConnectionAsync();
        user = await _userRepository.GetByActiveDirectoryGuidAsync(ldapResult.Guid.Value, connection);

        if (user is null)
        {
            if (!ldapResult.ShouldCreate)
            {
                return null;
            }

            // User does not exist in the database, create a new user
            user = new User()
            {
                ActiveDirectoryGuid = ldapResult.Guid.Value,
                Username = username.ToUpper(),
                Email = ldapResult.Attributes["mail"],
                DisplayName = $"{ldapResult.Attributes["givenName"]} {ldapResult.Attributes["sn"]}",
                AccountType = AccountType.ActiveDirectory,
                IsActive = true,
                LockoutEnabled = true,
            };

            await _userRepository.CreateAsync(user, connection);

            return user;
        }

        // Update attributes from Active Directory
        user.Email = ldapResult.Attributes["mail"];
        user.DisplayName = $"{ldapResult.Attributes["givenName"]} {ldapResult.Attributes["sn"]}";
        user.Username = username.ToUpper();
        user.AccessFailedCount = 0;

        await _userRepository.UpdateAsync(user, connection);

        return user;
    }

    /// <summary>
    /// Verifies the provided username and password against the local user store.
    /// The supplied password is combined with the user's stored salt and verified
    /// using the ASP.NET Core <see cref="PasswordHasher{TUser}"/>.
    /// </summary>
    /// <param name="username">The username to verify.</param>
    /// <param name="password">The plaintext password to verify.</param>
    /// <returns>The authenticated <see cref="User"/> on success; otherwise <c>null</c>.</returns>
    public async Task<User?> LoginAsync(string username, string password)
    {
        User? user;

        using var connection = await _dbFactory.CreateConnectionAsync();
        user = await _userRepository.GetByUsernameAsync(username, connection);
        if (user is not null)
        {
            PasswordHasher<User> hasher = new();
            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, password + user.Salt);

            if (result is PasswordVerificationResult.Failed)
            {
                user = null;
            }
        }

        return user;
    }
}
