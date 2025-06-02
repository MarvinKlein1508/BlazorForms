namespace BlazorForms.Application.Common.Interfaces;

/// <summary>
/// Helper interface to provide properties which can be used within the interface <see cref="ILocalizedDbModel{T}"/>
/// </summary>
public interface ILocalizationHelper
{
    public string GetLanguageCode();
}