namespace FormPortal.Core.Interfaces
{
    /// <summary>
    /// Helper interface to provide properties which can be used within the interface <see cref="ILocalizedDbModel{T}"/>
    /// </summary>
    public interface ILocalizationHelper
    {
        /// <summary>
        /// Gets or sets the ISO 639-1 two letter code for the language
        /// </summary>
        string Code { get; set; }
    }
}
