using DbUp;

namespace BlazorForms.Core.Database;

public class DbInitializer
{
    private readonly string _connectionString;

    public DbInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<int> InitializeAsync()
    {
        EnsureDatabase.For.MySqlDatabase(_connectionString);

        var upgrader = DeployChanges.To.MySqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DbInitializer).Assembly)
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                return Task.FromResult(-1);
            }
        }

        return Task.FromResult(0);
    }
}
