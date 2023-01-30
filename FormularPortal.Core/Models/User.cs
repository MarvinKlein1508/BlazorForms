namespace FormularPortal.Core.Models
{
    public sealed class User
    {
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("username")]
        public string Username { get; set; } = string.Empty;
        [CompareField("displayname")]
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
    }
}
