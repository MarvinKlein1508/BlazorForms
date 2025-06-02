using BlazorForms.Application.Domain;
using System.Globalization;

namespace BlazorForms.Application.Common;

public abstract class LocalizationModelBase<T> where T : ILocalizationHelper
{
    public List<T> Descriptions { get; set; } = [];
    public T? GetLocalization(CultureInfo culture)
    {
        var description = Descriptions.FirstOrDefault(x => x.GetLanguageCode().Equals(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase));
        return description;
    }
}