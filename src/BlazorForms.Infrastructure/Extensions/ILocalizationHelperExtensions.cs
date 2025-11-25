using System.Globalization;

namespace BlazorForms.Infrastructure.Extensions;

public static class ILocalizationHelperExtensions
{
    extension(ILocalizationHelper helper)
    {
        public CultureInfo ToCulture()
        {
            var culture = Storage.SupportedCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName.Equals(helper.GetLanguageCode(), StringComparison.OrdinalIgnoreCase));

            if (culture is null)
            {
                return Storage.SupportedCultures[0];
            }

            return culture;
        }
    }
}
