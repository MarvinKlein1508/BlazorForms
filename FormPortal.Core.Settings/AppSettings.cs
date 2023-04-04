namespace FormPortal.Core.Settings
{

    public class AppSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int PageLimit { get; set; } = 30;
        public LdapSettings LdapSettings { get; set; } = new();
        public EmailSettings EmailSettings { get; set; } = new();
    }

}