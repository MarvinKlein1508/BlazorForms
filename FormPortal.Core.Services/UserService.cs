using DatabaseControllerProvider;
using FormPortal.Core.Filters;
using FormPortal.Core.Interfaces;
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
    ); {dbController.GetLastIdSql()}";

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
            string sql = @"SELECT * FROM users WHERE UPPER(username) = UPPER(@USERNAME) AND origin = 'local'";

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

        public async Task<List<User>> GetAsync(UserFilter filter, IDbController dbController)
        {
            StringBuilder sqlBuilder = new();
            sqlBuilder.Append("SELECT * FROM users WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));
            sqlBuilder.AppendLine(@$"  ORDER BY user_id DESC");
            sqlBuilder.AppendLine(dbController.GetPaginationSyntax(filter.PageNumber, filter.Limit));

            // Zum Debuggen schreiben wir den Wert einmal als Variabel
            string sql = sqlBuilder.ToString();

            List<User> list = await dbController.SelectDataAsync<User>(sql, GetFilterParameter(filter));

            // Berechtigungen müssen noch geladen werden
            sql = @"SELECT * FROM permissions";
            List<Permission> permissions = await dbController.SelectDataAsync<Permission>(sql);

            sql = "SELECT * FROM user_permissions";
            List<UserPermission> user_permissions = await dbController.SelectDataAsync<UserPermission>(sql);

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
        UPPER(anzeigename) LIKE @SEARCHPHRASE
    OR  UPPER(email) LIKE @SEARCHPHRASE
    OR  UPPER(username) LIKE @SEARCHPHRASE
)");
            }


            string sql = sb.ToString();
            return sql;
        }

        public async Task<int> GetTotalAsync(UserFilter filter, IDbController dbController)
        {
            StringBuilder sqlBuilder = new();
            sqlBuilder.AppendLine("SELECT COUNT(*) FROM users WHERE 1 = 1");
            sqlBuilder.AppendLine(GetFilterWhere(filter));

            string sql = sqlBuilder.ToString();

            int result = await dbController.GetFirstAsync<int>(sql, GetFilterParameter(filter));

            return result;
        }

        public Task UpdateAsync(User input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User input, User oldInputToCompare, IDbController dbController)
        {
            throw new NotImplementedException();
        }
    }
}
