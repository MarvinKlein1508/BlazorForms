using BlazorForms.Core.Interfaces;
using System.Globalization;

namespace BlazorForms.Core.Models;

public abstract class LocalizationModelBase<T> where T : ILocalizationHelper
{
    public List<T> Description { get; set; } = [];
    public T? GetLocalization(CultureInfo culture)
    {
        var description = Description.FirstOrDefault(x => x.Code.Equals(culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase));
        return description;
    }
}
