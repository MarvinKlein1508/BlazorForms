using System.Data;
using System.Globalization;
using System.Security.Claims;
using BlazorForms.Infrastructure.Database;
using BlazorForms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Provides helper methods to access the currently authenticated user and role information.
/// </summary>
/// <remarks>
/// This service depends on an <see cref="AuthenticationStateProvider"/> to obtain the current
/// authentication state and a <see cref="UserRepository"/> to load user details from the database.
/// </remarks>
public class AuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IDbConnectionFactory _dbFactory;
    private readonly UserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="authenticationStateProvider">
    /// The authentication state provider used to obtain the current <see cref="ClaimsPrincipal"/>.
    /// </param>
    /// <param name="dbFactory">Factory used to create database connections when needed.</param>
    /// <param name="userRepository">Repository used to read <see cref="User"/> entities.</param>
    public AuthService(AuthenticationStateProvider authenticationStateProvider, IDbConnectionFactory dbFactory, UserRepository userRepository)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _dbFactory = dbFactory;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Gets the currently authenticated <see cref="User"/> by reading the authentication state and querying the database.
    /// </summary>
    /// <param name="connection">
    /// Optional open <see cref="IDbConnection"/> to use for the query. If <c>null</c>, a new connection will be created
    /// using the configured <see cref="IDbConnectionFactory"/> and disposed by this method.
    /// </param>
    /// <returns>
    /// The matched <see cref="User"/> if the user is authenticated and a corresponding database record exists; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method expects an authentication claim named "userId" containing the numeric user identifier.
    /// Callers that provide a connection remain responsible for disposing it; when no connection is provided,
    /// the method will create and dispose its own connection.
    /// </remarks>
    public async Task<User?> GetUserAsync(IDbConnection? connection = null)
    {

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity is not null && user.Identity.IsAuthenticated)
        {
            Claim? claim = user.FindFirst("userId");
            if (claim is null)
            {
                return null;
            }

            var userId = Convert.ToInt32(claim.Value, CultureInfo.InvariantCulture);
            bool shouldDispose = connection is null;
            connection ??= await _dbFactory.CreateConnectionAsync();

            var result = await _userRepository.GetAsync(userId, connection);

            if (shouldDispose)
            {
                connection.Dispose();
            }

            return result;
        }

        return null;
    }

    /// <summary>
    /// Determines whether the current authenticated user is in the specified role.
    /// </summary>
    /// <param name="roleName">The role name to check.</param>
    /// <returns>
    /// A <see cref="Task{Boolean}"/> representing whether the user is in the role.
    /// If no user is authenticated, this returns <c>false</c>.
    /// </returns>
    public async Task<bool> HasRole(string roleName)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        return user.IsInRole(roleName);
    }

    /// <summary>
    /// Checks membership for multiple role names for the currently authenticated user.
    /// </summary>
    /// <param name="roleNames">An array of role names to check.</param>
    /// <returns>
    /// A dictionary mapping each provided role name to a boolean indicating membership (<c>true</c> if the user is in the role).
    /// If no user is authenticated, all values will be <c>false</c>.
    /// </returns>
    public async Task<Dictionary<string, bool>> CheckRoles(params string[] roleNames)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        Dictionary<string, bool> results = [];

        foreach (var roleName in roleNames)
        {
            results.Add(roleName, user.IsInRole(roleName));
        }

        return results;
    }
}
