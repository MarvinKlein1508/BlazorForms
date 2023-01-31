using DatabaseControllerProvider;

namespace FormularPortal.Core.Models
{
    public class Permission : IDbModel
    {
        [CompareField("permission_id")]
        public int PermissionId { get; set; }
        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("identifier")]
        public string Identifier { get; set; } = string.Empty;
        [CompareField("description")]
        public string Description { get; set; } = string.Empty;

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "PERMISSION_ID", PermissionId },
                { "NAME", Name },
                { "IDENTIFIER", Identifier },
                { "DESCRIPTION", Description }
            };
        }
    }
}
