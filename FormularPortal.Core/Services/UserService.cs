using FormularPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
{
    public class UserService : IModelService<User, int>
    {
        public async Task CreateAsync(User input, SqlController sqlController)
        {
            string sql = @"INSERT INTO users
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
) SELECT SCOPE_IDENTITY();";

            input.UserId = await sqlController.GetFirstAsync<int>(sql, new
            {
                USERNAME = input.Username,
                DISPLAY_NAME = input.DisplayName,
                ACTIVE_DIRECTORY_GUID = input.ActiveDirectoryGuid,
                EMAIL = input.Email,
                PASSWORD = input.Password,
                SALT = input.Salt,
                ORIGIN = input.Origin
            });
        }

        public async Task DeleteAsync(User input, SqlController sqlController)
        {
            string sql = "DELETE FROM users WHERE user_id = @USER_ID";

            await sqlController.QueryAsync(sql, new
            {
                USER_ID = input.UserId,
            });
        }

        public async Task<User?> GetAsync(int userId, SqlController sqlController)
        {
            string sql = @"SELECT * FROM users WHERE user_id = @USER_ID";

            var user = await sqlController.GetFirstAsync<User>(sql, new
            {
                USER_ID = userId
            });

            if (user is not null)
            {
                //user.Berechtigungen = await GetUserBerechtigungAsync(user, sqlController);
            }

            return user;
        }
        public async Task<User?> GetAsync(Guid guid, SqlController sqlController)
        {
            string sql = @"SELECT * FROM users WHERE active_directory_guid = @ACTIVE_DIRECTORY_GUID AND origin = 'ad'";

            var user = await sqlController.GetFirstAsync<User>(sql, new
            {
                ACTIVE_DIRECTORY_GUID = guid
            });

            if (user is not null)
            {
                //user.Berechtigungen = await GetUserBerechtigungAsync(user, sqlController);
            }
            return user;
        }
        public Task UpdateAsync(User input, SqlController sqlController)
        {
            throw new NotImplementedException();
        }
    }
}
