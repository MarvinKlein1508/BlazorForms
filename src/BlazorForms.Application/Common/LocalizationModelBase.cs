using System.Globalization;

namespace BlazorForms.Application.Common;

public abstract class LocalizationModelBase<T> where T : ILocalizationHelper
{
    public List<T> Description { get; set; } = [];
    public T? GetLocalization(int languageId)
    {
        var description = Description.FirstOrDefault(x => x.GetLanguageId() == languageId);
        return description;
    }
}