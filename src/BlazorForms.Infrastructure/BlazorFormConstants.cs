namespace BlazorForms.Infrastructure;

public static class BlazorFormConstants
{
    /// <summary>
    /// Temporary workaround for an issue in FluentUI Blazor.
    /// Introduces a short delay to allow the components to fully load their values
    /// before the UI is updated.
    /// https://github.com/microsoft/fluentui-blazor/issues/4743
    /// </summary>
    public const int FLUENTUI_SEARCH_DELAY_IN_MS = 5;
}
