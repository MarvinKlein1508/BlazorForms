using DbController;
using FormPortal.Core.Filters;
using FormPortal.Core.Models;
using System.Text;

namespace FormPortal.Core.Services
{
    public class UserService : IModelService<User, int, UserFilter>
    {
        private readonly PermissionService _permissionService;

        public UserService(PermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        public async Task CreateAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
    ); {dbController.GetLastIdSql()}";

            input.UserId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

            await _permissionService.UpdateUserPermissionsAsync(input, dbController, cancellationToken);
        }

        public async Task DeleteAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = "DELETE FROM users WHERE user_id = @USER_ID";

            await dbController.QueryAsync(sql, new
            {
                USER_ID = input.UserId,
            }, cancellationToken);
        }

        public async Task<User?> GetAsync(int userId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"SELECT * FROM users WHERE user_id = @USER_ID";

            var user = await dbController.GetFirstAsync<User>(sql, new
            {
                USER_ID = userId
            }, cancellationToken);

            if (user is not null)
            {
                user.Permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, dbController, cancellationToken);
            }

            return user;
        }
        public async Task<User?> GetAsync(Guid guid, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"SELECT * FROM users WHERE active_directory_guid = @ACTIVE_DIRECTORY_GUID AND origin = 'ad'";

            var user = await dbController.GetFirstAsync<User>(sql, new
            {
                ACTIVE_DIRECTORY_GUID = guid
            }, cancellationToken);

            if (user is not null)
            {
                user.Permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, dbController, cancellationToken);
            }
            return user;
        }
        public async Task<User?> GetAsync(string username, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"SELECT * FROM users WHERE UPPER(username) = UPPER(@USERNAME) AND origin = 'local'";

            var user = await dbController.GetFirstAsync<User>(sql, new
            {
                USERNAME = username
            }, cancellationToken);

            if (user is not null)
            {
                user.Permissions = await _permissionService.GetUserPermissionsAsync(user.UserId, dbController, cancellationToken);
            }

            return user;
        }

        public async Task<List<User>> GetAsync(UserFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder sqlBuilder = new();
            sqlBuilder.Append("SELECT * FROM users WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));
            sqlBuilder.AppendLine(@$"  ORDER BY user_id DESC");
            sqlBuilder.AppendLine(dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit));

            // Zum Debuggen schreiben wir den Wert einmal als Variabel
            string sql = sqlBuilder.ToString();

            List<User> list = await dbController.SelectDataAsync<User>(sql, GetFilterParameter(filter), cancellationToken);

            // Berechtigungen müssen noch geladen werden
            List<Permission> permissions = await PermissionService.GetAllAsync(dbController);

            sql = "SELECT * FROM user_permissions";
            List<UserPermission> user_permissions = await dbController.SelectDataAsync<UserPermission>(sql, null, cancellationToken);

            foreach (var user in list)
            {
                List<UserPermission> permissions_for_user = user_permissions.Where(x => x.UserId == user.UserId).ToList();
                List<int> permission_ids = permissions_for_user.Select(x => x.PermissionId).ToList();

                user.Permissions = permissions.Where(x => permission_ids.Contains(x.PermissionId)).ToList();
            }



            return list;
        }

        public Dictionary<string, object?> GetFilterParameter(UserFilter filter)
        {
            return new Dictionary<string, object?>
            {
                { "SEARCHPHRASE", $"%{filter.SearchPhrase}%" }
            };
        }

        public string GetFilterWhere(UserFilter filter)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
            {
                sb.AppendLine(@" AND 
(
        UPPER(display_name) LIKE @SEARCHPHRASE
    OR  UPPER(email) LIKE @SEARCHPHRASE
    OR  UPPER(username) LIKE @SEARCHPHRASE
)");
            }

            if (filter.BlockedIds.Any())
            {
                sb.AppendLine($" AND user_id NOT IN ({string.Join(",", filter.BlockedIds)})");
            }


            string sql = sb.ToString();
            return sql;
        }

        public async Task<int> GetTotalAsync(UserFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder sqlBuilder = new();
            sqlBuilder.AppendLine("SELECT COUNT(*) FROM users WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));

            string sql = sqlBuilder.ToString();

            int result = await dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter), cancellationToken);

            return result;
        }

        public async Task UpdateAsync(User input, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"UPDATE users SET
username = @USERNAME,
display_name = @DISPLAY_NAME,
email = @EMAIL
WHERE user_id = @USER_ID";

            await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);

            await _permissionService.UpdateUserPermissionsAsync(input, dbController, cancellationToken);
        }
        public static async Task<bool> FirstUserExistsAsync(IDbController dbController)
        {
            string sql = "SELECT * FROM users";

            var tmp = await dbController.GetFirstAsync<object>(sql);

            return tmp != null;
        }
    }
}
