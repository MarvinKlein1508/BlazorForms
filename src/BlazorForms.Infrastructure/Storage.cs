using BlazorForms.Infrastructure.Database;
using BlazorForms.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;

namespace BlazorForms.Infrastructure;

public static class Storage
{
    private static IConfiguration? _configuration;
    private static readonly Dictionary<Type, object> _storage = [];
    public static List<CultureInfo> SupportedCultures { get; private set; } = [];
    public static string[] SupportedCultureCodes => SupportedCultures.Select(x => x.Name).ToArray();
    public static async Task InitAsync(IConfiguration configuration)
    {
        _configuration = configuration;
        string connectionString = configuration.GetConnectionString("Default") ?? throw new ArgumentNullException(nameof(connectionString));

        var dbFactory = new NpgsqlConnectionFactory(connectionString);
        using var connection = await dbFactory.CreateConnectionAsync();

        _storage.Add(typeof(Language), await LanguageRepository.GetAllAsync(connection));
        _storage.Add(typeof(FormStatus), await FormStatusRepository.GetAllAsync(connection));
        //_storage.Add(typeof(UserGroup), await RoleRepository.GetAllAsync(connection));

        foreach (var language in Get<Language>())
        {
            try
            {
                // Could crash here when there is an invalid language code within the database.
                SupportedCultures.Add(new CultureInfo(language.Code));
            }
            catch (Exception)
            {

            }
        }
    }

    /// <summary>
    /// Creates or updates an object in the corresponding list fro the type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    public static void UpdateStorage<T, TIdentifier>(T input) where T : class, IDbModel<TIdentifier>
    {
        if (!_storage.ContainsKey(typeof(T)))
        {
            return;
        }

        if (_storage[typeof(T)] is not List<T> list)
        {
            return;
        }

        var existingItem = list.FirstOrDefault(x => x?.GetIdentifier()?.Equals(input.GetIdentifier()) ?? false);

        if (existingItem == null)
        {
            list.Add(input);
        }
        else
        {
            int index = list.IndexOf(existingItem);
            list[index] = input;
        }
    }

    /// <summary>
    /// Deletes an item from the corresponding object list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    public static void DeleteFromStorage<T, TIdentifier>(T input) where T : class, IDbModel<TIdentifier>
    {
        var storage = _storage.GetValueOrDefault(typeof(T)) as List<T>;

        var item = storage?.Cast<T>().FirstOrDefault(x => x.GetIdentifier()?.Equals(input.GetIdentifier()) ?? false);

        if (item is not null)
        {
            storage!.Remove(item);
        }
    }

    /// <summary>
    /// Gets the corresponding list for the type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>This method never returns null. When no list for <see cref="T"/> is specified, it returns a new empty list</returns>
    public static IEnumerable<T> Get<T>() where T : class
    {
        if (_storage.ContainsKey(typeof(T)))
        {
            var storage = _storage.GetValueOrDefault(typeof(T)) as List<T> ?? [];

            foreach (var item in storage)
            {
                yield return (T)item;
            }
        }
    }

    /// <summary>
    /// Gets the corresponding item for the specified identifier.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <typeparam name="TIdentifier">The type of identifier.</typeparam>
    /// <param name="identifier">The identifier of the item to retrieve.</param>
    /// <returns>When found, this method returns an item of type <see cref="T"/>, otherwise it returns null.</returns>
    public static T? Get<T, TIdentifier>(TIdentifier identifier) where T : class, IDbModel<TIdentifier>
    {
        if (identifier is null)
        {
            return null;
        }

        foreach (var item in Get<T>())
        {
            if (item.GetIdentifier()?.Equals(identifier) ?? false)
            {
                return item;
            }
        }

        return null;
    }
}
