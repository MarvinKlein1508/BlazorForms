using DatabaseControllerProvider;

namespace FormPortal.Core.Models
{
    public sealed class UserPermission
    {
        [CompareField("user_id")]
        public int UserId { get; set; }
        [CompareField("permission_id")]
        public int PermissionId { get; set; }
    }
}
