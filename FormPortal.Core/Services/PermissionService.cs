using DbController;
using FormPortal.Core.Models;

namespace FormPortal.Core.Services
{
    public class PermissionService
    {
        public async Task<List<Permission>> GetUserPermissionsAsync(int userId, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string sql = @"SELECT p.*
    FROM user_permissions up
    INNER JOIN permissions p ON (p.permission_id = up.permission_id)
    WHERE user_id = @USER_ID";

            var list = await dbController.SelectDataAsync<Permission>(sql, new
            {
                USER_ID = userId
            }, cancellationToken);

            await LoadPermissionDescriptionsAsync(list, dbController, cancellationToken);

            return list;
        }

        public async Task UpdateUserPermissionsAsync(User user, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Step 1: Delete all permissions for the user.
            string sql = "DELETE FROM user_permissions WHERE user_id = @USER_ID";
            await dbController.QueryAsync(sql, new
            {
                USER_ID = user.UserId
            }, cancellationToken);

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
                }, cancellationToken);

            }
        }
        public static async Task<List<Permission>> GetAllAsync(IDbController dbController)
        {
            string sql = "SELECT * FROM permissions";

            var list = await dbController.SelectDataAsync<Permission>(sql);
            await LoadPermissionDescriptionsAsync(list, dbController);
            return list;
        }

        private static async Task LoadPermissionDescriptionsAsync(List<Permission> list, IDbController dbController, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (list.Any())
            {
                IEnumerable<int> permissionIds = list.Select(x => x.Id);
                string sql = $"SELECT * FROM permission_description WHERE permission_id IN ({string.Join(",", permissionIds)})";
                List<PermissionDescription> descriptions = await dbController.SelectDataAsync<PermissionDescription>(sql, null, cancellationToken);

                foreach (var permission in list)
                {
                    permission.Description = descriptions.Where(x => x.PermissionId == permission.Id).ToList();
                }
            }
        }
    }
}
