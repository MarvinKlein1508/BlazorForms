namespace DatabaseControllerProvider
{
    public interface IDbProviderService
    {
        IDbController GetDbController(string connectionString);
    }
}