using BlazorForms.Core.Interfaces;
using BlazorForms.Core.Models;
using BlazorForms.Core.Models.FormElements;
using BlazorForms.Core.Services;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace BlazorForms.Core;

public static class Storage
{
    private static IConfiguration? _configuration;
    private readonly static Dictionary<Type, object> _storage = [];

    public static bool FirstUserExists { get; set; }
    public static List<CultureInfo> SupportedCultures { get; set; } = [];
    public static string[] SupportedCultureCodes => SupportedCultures.Select(x => x.Name).ToArray();
    private static List<(string name, Func<Dictionary<Type, object>, IDbController, Task> configure)> Provider { get; } = [];
    public static List<FormElement> Elements { get; } =
    [
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
    ];

    public static bool IsLdapLoginEnabled => _configuration?.GetSection("LdapSettings").GetValue<bool>("ENABLE_LDAP_LOGIN") ?? false;
    public static bool IsLocalLoginEnabled => _configuration?.GetSection("LdapSettings").GetValue<bool>("ENABLE_LOCAL_LOGIN") ?? false;
    public static string LdapServer => _configuration?["LdapSettings:LDAP_SERVER"] ?? string.Empty;
    public static string LdapDomainServer => _configuration?["LdapSettings:DOMAIN_SERVER"] ?? string.Empty;
    public static string LdapDistinguishedName => _configuration?["LdapSettings:DistinguishedName"] ?? string.Empty;

    public static Dictionary<string, string> MimeTypes => _configuration?.GetSection("MimeTypes").GetChildren().ToDictionary(x => x.Key, x => x.Value!) ?? [];
    public static int PageLimit => _configuration?.GetValue<int>("PageLimit") ?? 30;

    public static void RegisterProvider(string name, Func<Dictionary<Type, object>, IDbController, Task> provider)
    {
        Provider.Add((name, provider));
    }

    public static async Task InitAsync(IConfiguration configuration)
    {
        _configuration = configuration;

        using IDbController dbController = new MySqlController(_configuration.GetConnectionString("Default")!);


        _storage.Add(typeof(Language), await LanguageService.GetAllAsync(dbController));
        _storage.Add(typeof(Permission), await PermissionService.GetAllAsync(dbController));
        _storage.Add(typeof(FormStatus), await FormStatusService.GetAllAsync(dbController));

        FirstUserExists = await UserService.FirstUserExistsAsync(dbController);

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


        if (SupportedCultures.Count == 0)
        {
            throw new InvalidOperationException("No supported language could be found.");
        }

        foreach (var provider in Provider)
        {
            await provider.configure(_storage, dbController);
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

        var list = _storage[typeof(T)] as List<T>;

        if (list is null)
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

    public static Language GetActiveLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var language = Get<Language>().FirstOrDefault(x => x.Code == currentCulture.TwoLetterISOLanguageName);

        return language is null ? Get<Language>().First() : language;
    }
    public static CultureInfo ToCulture(this ILocalizationHelper helper)
    {
        var culture = SupportedCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName.Equals(helper.Code, StringComparison.OrdinalIgnoreCase));

        if (culture is null)
        {
            return SupportedCultures[0];
        }

        return culture;
    }
}
