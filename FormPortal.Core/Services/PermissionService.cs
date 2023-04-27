using DbController;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class PermissionService : IModelService<Permission, int>
    {
        public Task CreateAsync(Permission input, IDbController dbController) => throw new NotImplementedException();
        public Task DeleteAsync(Permission input, IDbController dbController) => throw new NotImplementedException();
        public Task UpdateAsync(Permission input, IDbController dbController) => throw new NotImplementedException();

        public async Task<Permission?> GetAsync(int permissionId, IDbController dbController)
        {
            string sql = "SELECT * FROM permissions WHERE permission_id = @PERMISSION_ID";

            var item = await dbController.GetFirstAsync<Permission>(sql, new
            {
                PERMISSION_ID = permissionId
            });

            if (item is not null)
            {
                sql = "SELECT * FROM permission_description WHERE permission_id = @PERMISSION_ID";

                item.Description = await dbController.SelectDataAsync<PermissionDescription>(sql, new
                {
                    PERMISSION_ID = permissionId
                });
            }

            return item;
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

            await LoadPermissionDescriptionsAsync(list, dbController);

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
            await LoadPermissionDescriptionsAsync(list, dbController);
            return list;
        }

        private static async Task LoadPermissionDescriptionsAsync(List<Permission> list, IDbController dbController)
        {
            if (list.Any())
            {
                IEnumerable<int> permissionIds = list.Select(x => x.Id);
                string sql = $"SELECT * FROM permission_description WHERE permission_id IN ({string.Join(",", permissionIds)})";
                List<PermissionDescription> descriptions = await dbController.SelectDataAsync<PermissionDescription>(sql);

                foreach (var permission in list)
                {
                    permission.Description = descriptions.Where(x => x.PermissionId == permission.Id).ToList();
                }
            }
        }
    }
}
