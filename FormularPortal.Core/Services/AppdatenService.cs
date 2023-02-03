using DatabaseControllerProvider;
using FormularPortal.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormularPortal.Core.Services
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

        public static async Task InitAsync(IConfiguration configuration, DbProviderService dbProviderService)
        {
            _configuration = configuration;

            using IDbController dbController = dbProviderService.GetDbController(DbProvider, ConnectionString);
            Permissions = await PermissionService.GetAllAsync(dbController);

            // TODO: Init FirstUserExists
        }
        public static string DbProvider => _configuration?["DbProvider"] ?? string.Empty;
        public static string ConnectionString => _configuration?.GetConnectionString("Default") ?? string.Empty;
        public static bool IsLdapLoginEnabled => _configuration?.GetSection("Login").GetValue<bool>("ENABLE_LDAP_LOGIN") ?? false;
        public static bool IsLocalLoginEnabled => _configuration?.GetSection("Login").GetValue<bool>("ENABLE_LOCAL_LOGIN") ?? false;
        public static string LdapServer => _configuration?["Login:LDAP_SERVER"] ?? string.Empty;
        public static string LdapDomainServer => _configuration?["Login:DOMAIN_SERVER"] ?? string.Empty;
        public static string LdapDistinguishedName => _configuration?["Login:DistinguishedName"] ?? string.Empty;

    }
}
