using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public class PermissionService : IModelService<Permission, int>
    {
        public Task CreateAsync(Permission input, SqlController sqlController)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Permission input, SqlController sqlController)
        {
            throw new NotImplementedException();
        }

        public Task<Permission?> GetAsync(int permissionId, SqlController sqlController)
        {
            string sql = "SELECT * FROM permissions WHERE permission_id = @PERMISSION_ID";

            var item = sqlController.GetFirstAsync<Permission>(sql, new
            {
                PERMISSION_ID = permissionId
            });

            return item;
        }

        public Task UpdateAsync(Permission input, SqlController sqlController)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId, SqlController sqlController)
        {
            string sql = @"SELECT p.*
FROM user_permissions up
INNER JOIN permissions p ON (p.permission_id = up.permission_id)
WHERE user_id = @USER_ID";

            var list = await sqlController.SelectDataAsync<Permission>(sql, new
            {
                USER_ID = userId
            });

            return list;
        }

        public async Task UpdateUserPermissionsAsync(User user, SqlController sqlController)
        {
            // Step 1: Delete all permissions for the user.
            string sql = "DELETE FROM user_permissions WHERE user_id = @USER_ID";
            await sqlController.QueryAsync(sql, new
            {
                USER_ID = user.UserId
            });

            // Step 2: Add all permissions from the object back.
            foreach (var permission in user.Permissions)
            {
                sql = @"INSERT INTO user_permissions
(
user_id,
permission_id
)
VALUES
(
@USER_ID,
@PERMISSION_ID
)";

                await sqlController.QueryAsync(sql, new
                {
                    USER_ID = user.UserId,
                    PERMISSION_ID = permission.PermissionId
                });

            }
        }
        public static async Task<List<Permission>> GetAllAsync(SqlController sqlController)
        {
            string sql = "SELECT * FROM permissions";

            var list = await sqlController.SelectDataAsync<Permission>(sql);

            return list;
        }
    }
}
