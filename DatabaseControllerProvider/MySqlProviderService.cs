namespace DatabaseControllerProvider
{
    public sealed class MySqlProviderService : IDbProviderService
    {
        public IDbController GetDbController(string connectionString) => new MySqlController(connectionString);
    }
}
