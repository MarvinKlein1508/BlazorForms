namespace FormPortal.Core.Settings
{
    public class LdapSettings
    {
        public bool ENABLE_LDAP_LOGIN { get; set; }
        public bool ENABLE_LOCAL_LOGIN { get; set; }
        public string LDAP_SERVER { get; set; } = string.Empty;
        public string DOMAIN_SERVER { get; set; } = string.Empty;
        public string DistinguishedName { get; set; } = string.Empty;
    }

}