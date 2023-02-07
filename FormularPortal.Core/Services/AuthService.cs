using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FormularPortal.Core.Services
{
    public class AuthService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserService _userService;
        private readonly DbProviderService _dbProviderService;

        public AuthService(AuthenticationStateProvider authenticationStateProvider, UserService userService, DbProviderService dbProviderService)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userService = userService;
            _dbProviderService = dbProviderService;
        }
        /// <summary>
        /// Converts the active claims into a <see cref="User"/> object
        /// </summary>
        /// <returns></returns>
        public async Task<User?> GetUserAsync()
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

                var userId = Convert.ToInt32(claim.Value);

                using IDbController dbController = _dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
                return await _userService.GetAsync(userId, dbController);
            }

            return null;
        }
        /// <summary>
        /// Checks if the currently logged in user as a specific role within it's claims.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<bool> HasRole(string roleName)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            return user.IsInRole(roleName);
        }
    }
}
