using System.Globalization;

namespace BlazorForms.Infrastructure;

public static class BlazorFormConstants
{
    public static class Roles
    {
        public const string Admin = "ADMIN";
        public const string Editor = "EDITOR";
        public const string User = "USER";
    }
    /// <summary>
    /// Temporary workaround for an issue in FluentUI Blazor.
    /// Introduces a short delay to allow the components to fully load their values
    /// before the UI is updated.
    /// https://github.com/microsoft/fluentui-blazor/issues/4743
    /// </summary>
    public const int FLUENTUI_SEARCH_DELAY_IN_MS = 5;

    public static CultureInfo[] SupportedCultures { get; private set; } = [];
    public static string[] SupportedCultureCodes => SupportedCultures.Select(x => x.Name).ToArray();

    static BlazorFormConstants()
    {
        SupportedCultures =
        [
            new CultureInfo("en"),
            new CultureInfo("de")
        ];
    }
}
