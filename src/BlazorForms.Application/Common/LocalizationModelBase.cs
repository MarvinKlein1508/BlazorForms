using System.Globalization;

namespace BlazorForms.Application.Common;

public abstract class LocalizationModelBase<T> where T : ILocalizationHelper
{
    public List<T> Descriptions { get; set; } = [];
    public T? GetLocalization(int languageId)
    {
        var description = Descriptions.FirstOrDefault(x => x.GetLanguageId() == languageId);
        return description;
    }
}