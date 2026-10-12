using Microsoft.AspNetCore.Components;

namespace BlazorForms.Infrastructure.Auth;

/// <summary>
/// Provides safe navigation helpers that centralize redirect logic for the application.
/// </summary>
/// <param name="navigationManager">The <see cref="NavigationManager"/> used to perform navigations.</param>
public sealed class RedirectManager(NavigationManager navigationManager)
{
    /// <summary>
    /// Navigates to the specified <paramref name="uri"/> while protecting against open redirects.
    /// </summary>
    /// <param name="uri">
    /// The target URI. If <c>null</c>, an empty string is used which navigates to the application root.
    /// The method ensures the final value is a relative path to prevent open-redirect attacks.
    /// </param>
    public void RedirectTo(string? uri)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        navigationManager.NavigateTo(uri);
    }

    /// <summary>
    /// Navigates to the specified <paramref name="uri"/> while adding or replacing query parameters.
    /// The constructed URI is passed through the same safety checks as <see cref="RedirectTo(string?)"/>.
    /// </summary>
    /// <param name="uri">The base URI to navigate to. This can be relative or absolute.</param>
    /// <param name="queryParameters">
    /// A dictionary of query parameter names and values to apply to the <paramref name="uri"/>.
    /// Values may be <c>null</c> to indicate parameters without values.
    /// </param>
    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }
}
