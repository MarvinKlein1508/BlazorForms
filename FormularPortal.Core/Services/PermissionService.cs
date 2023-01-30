using DatabaseControllerProvider;
using FormularPortal.Core.Models;

namespace FormularPortal.Core.Services
{
    public class PermissionService : IModelService<Permission, int>
    {
        public Task CreateAsync(Permission input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Permission input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public Task<Permission?> GetAsync(int permissionId, IDbController dbController)
        {
            string sql = "SELECT * FROM permissions WHERE permission_id = @PERMISSION_ID";

            var item = dbController.GetFirstAsync<Permission>(sql, new
            {
                PERMISSION_ID = permissionId
            });

            return item;
        }

        public Task UpdateAsync(Permission input, IDbController dbController)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Permission>> GetUserPermissionsAsync(int userId, IDbController dbController)
        {
            string sql = @"SELECT p.*
    FROM user_permissions up
    INNER JOIN permissions p ON (p.permission_id = up.permission_id)
    WHERE user_id = @USER_ID";

            var list = await dbController.SelectDataAsync<Permission>(sql, new
            {
                USER_ID = userId
            });

            return list;
        }

        public async Task UpdateUserPermissionsAsync(User user, IDbController dbController)
        {
            // Step 1: Delete all permissions for the user.
            string sql = "DELETE FROM user_permissions WHERE user_id = @USER_ID";
            await dbController.QueryAsync(sql, new
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

                await dbController.QueryAsync(sql, new
                {
                    USER_ID = user.UserId,
                    PERMISSION_ID = permission.PermissionId
                });

            }
        }
        public static async Task<List<Permission>> GetAllAsync(IDbController dbController)
        {
            string sql = "SELECT * FROM permissions";

            var list = await dbController.SelectDataAsync<Permission>(sql);

            return list;
        }
    }
}
