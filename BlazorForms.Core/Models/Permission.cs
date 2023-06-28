using DbController;
using BlazorForms.Core.Interfaces;

namespace BlazorForms.Core.Models
{
    public class Permission : LocalizationModelBase<PermissionDescription>
    {
        [CompareField("permission_id")]
        public int PermissionId { get; set; }

        [CompareField("identifier")]
        public string Identifier { get; set; } = string.Empty;
        public int Id => PermissionId;



        public IEnumerable<Dictionary<string, object?>> GetLocalizedParameters()
        {
            foreach (var description in Description)
            {
                yield return new Dictionary<string, object?>
                {
                    { "PERMISSION_ID", PermissionId },
                    { "NAME", description.Name },
                    { "DESCRIPTION", description.Description }
                };
            }
        }

        public Dictionary<string, object?> GetParameters()
        {
            return new Dictionary<string, object?>
            {
                { "PERMISSION_ID", PermissionId },
                { "IDENTIFIER", Identifier }
            };
        }
    }

    public class PermissionDescription : ILocalizationHelper
    {
        [CompareField("permission_id")]
        public int PermissionId { get; set; }

        [CompareField("code")]
        public string Code { get; set; } = string.Empty;

        [CompareField("name")]
        public string Name { get; set; } = string.Empty;
        [CompareField("description")]
        public string Description { get; set; } = string.Empty;
    }
}
