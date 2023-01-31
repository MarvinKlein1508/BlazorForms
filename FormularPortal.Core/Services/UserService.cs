using DatabaseControllerProvider;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Services
{
    public class UserService : IModelService<User, int>
    {
        private readonly PermissionService _permissionService;

        public UserService(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        public async Task CreateAsync(User input, IDbController dbController)
        {
            string sql = $@"INSERT INTO users
    (
    username,
    display_name,
    active_directory_guid,
    email,
    password,
    salt,
    origin
    )
    VALUES 
    (
    @USERNAME,
    @DISPLAY_NAME,
    @ACTIVE_DIRECTORY_GUID,
    @EMAIL,
    @PASSWORD,
    @SALT,
    @ORIGIN
    ) {dbController.GetLastIdSql()}";

            input.UserId = await dbController.GetFirstAsync<int>(sql, input.GetParameters());

            await _permissionService.UpdateUserPermissionsAsync(input, dbController);
        }

        public async Task DeleteAsync(User input, IDbController dbController)
        {
            string sql = "DELETE FROM users WHERE user_id = @USER_ID";

            await dbController.QueryAsync(sql, new
            {
                USER_ID = input.UserId,
            });
        }

        public async Task<User?> GetAsync(int userId, IDbController dbController)
        {
            string sql = @"SELECT * FROM users WHERE user_id = @USER_ID";

            var user = await dbController.GetFirstAsync<User>(sql, new
            {
                USER_ID = userId
            });

            if (user is not null)
            {
                user.Permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, dbController);
            }

            return user;
        }
        public async Task<User?> GetAsync(Guid guid, IDbController dbController)
        {
            string sql = @"SELECT * FROM users WHERE active_directory_guid = @ACTIVE_DIRECTORY_GUID AND origin = 'ad'";

            var user = await dbController.GetFirstAsync<User>(sql, new
            {
                ACTIVE_DIRECTORY_GUID = guid
            });

            if (user is not null)
            {
                user.Permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, dbController);
            }
            return user;
        }
        public async Task<User?> GetAsync(string username, IDbController dbController)
        {
            string sql = @"SELECT * FROM users WHERE UPPER(username) = UPPER(@USERNAME) AND herkunft = 'local'";

            var user = await dbController.GetFirstAsync<User>(sql, new
            {
                USERNAME = username
            });

            if (user is not null)
            {
                user.Permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, dbController);
            }

            return user;
        }
        public Task UpdateAsync(User input, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}
