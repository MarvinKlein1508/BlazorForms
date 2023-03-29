using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using Microsoft.Extensions.Configuration;

namespace FormPortal.Core.Services
{
    public static class AppdatenService
    {
        public static bool FirstUserExists { get; set; } = false;
        public static List<FormElement> Elements { get; } = new List<FormElement>
        {
            new FormTextElement { Name = "Text" },
            new FormDateElement { Name = "Date" },
            new FormSelectElement { Name = "Select" },
            new FormCheckboxElement { Name = "Checkbox"},
            new FormTextareaElement { Name = "Textarea" },
            new FormFileElement { Name = "File"},
            new FormRadioElement { Name = "Radio"},
            new FormNumberElement { Name ="Number"},
            new FormTableElement { Name = "Table"},
            new FormLabelElement { Name = "Label"}
        };

        public static List<Permission> Permissions { get; set; } = new();

        private static IConfiguration? _configuration;

        public static async Task InitAsync(IConfiguration configuration, IDbProviderService dbProviderService)
        {
            _configuration = configuration;
            using IDbController dbController = dbProviderService.GetDbController(ConnectionString);
            Permissions = await PermissionService.GetAllAsync(dbController);
            FirstUserExists = await UserService.FirstUserExistsAsync(dbController);
        }
        public static string ConnectionString => _configuration?["ConnectionString"] ?? string.Empty;
        public static bool IsLdapLoginEnabled => _configuration?.GetSection("LdapSettings").GetValue<bool>("ENABLE_LDAP_LOGIN") ?? false;
        public static bool IsLocalLoginEnabled => _configuration?.GetSection("LdapSettings").GetValue<bool>("ENABLE_LOCAL_LOGIN") ?? false;
        public static string LdapServer => _configuration?["LdapSettings:LDAP_SERVER"] ?? string.Empty;
        public static string LdapDomainServer => _configuration?["LdapSettings:DOMAIN_SERVER"] ?? string.Empty;
        public static string LdapDistinguishedName => _configuration?["LdapSettings:DistinguishedName"] ?? string.Empty;

        public static int PageLimit => _configuration?.GetValue<int>("PageLimit") ?? 30;
    }
}
