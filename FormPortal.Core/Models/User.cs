using DbController;

namespace FormPortal.Core.Models
{
    public sealed class User : IDbModel
    {
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("username")]
        public string Username { get; set; } = string.Empty;
        [CompareField("display_name")]
        public string DisplayName { get; set; } = string.Empty;
        [CompareField("active_directory_guid")]
        public Guid? ActiveDirectoryGuid { get; set; }
        [CompareField("email")]
        public string Email { get; set; } = string.Empty;
        [CompareField("password")]
        public string Password { get; set; } = string.Empty;
        [CompareField("salt")]
        public string Salt { get; set; } = string.Empty;
        [CompareField("origin")]
        public string Origin { get; set; } = string.Empty;

        public List<Permission> Permissions { get; set; } = new();
        public int Id => UserId;
        /// <summary>
        /// Gets or sets a flag for a form manager to receive an email for new form entries
        /// </summary>
        [CompareField("receive_email")]
        public bool EmailEnabled { get; set; }
        /// <summary>
        /// Used to identify form managers which are able to approve forms with approval <see cref="FormStatus"/>
        /// </summary>
        [CompareField("can_approve")]
        public bool CanApprove { get; set; }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "USER_ID", UserId },
                { "USERNAME", Username },
                { "DISPLAY_NAME", DisplayName },
                { "ACTIVE_DIRECTORY_GUID", ActiveDirectoryGuid?.ToString() },
                { "EMAIL", Email },
                { "PASSWORD", Password },
                { "SALT", Salt },
                { "ORIGIN", Origin },

            };
        }
    }
}
