using Microsoft.FluentUI.AspNetCore.Components.Utilities;
using Microsoft.JSInterop;

namespace BlazorForms.Web.Infrastructure;
public class CacheStorageAccessor(IJSRuntime js) : JSModule(js, "./_content/HSTrend.Components/js/CacheStorageAccessor.js")
{
    private string? CurrentCacheVersion;

    public async ValueTask PutAsync(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
    {
        var requestMethod = requestMessage.Method.Method;
        var requestBody = await GetRequestBodyAsync(requestMessage);
        var responseBody = await responseMessage.Content.ReadAsStringAsync();

        await InvokeVoidAsync("put", requestMessage.RequestUri!, requestMethod, requestBody, responseBody);
    }

    public async ValueTask<string> PutAndGetAsync(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
    {
        var requestMethod = requestMessage.Method.Method;
        var requestBody = await GetRequestBodyAsync(requestMessage);
        var responseBody = await responseMessage.Content.ReadAsStringAsync();

        await InvokeVoidAsync("put", requestMessage.RequestUri!, requestMethod, requestBody, responseBody);

        return responseBody;
    }

    public async ValueTask<string> GetAsync(HttpRequestMessage requestMessage)
    {
        if (CurrentCacheVersion is null)
        {
            await InitializeCacheAsync();
        }

        var result = await InternalGetAsync(requestMessage);

        return result;
    }
    private async ValueTask<string> InternalGetAsync(HttpRequestMessage requestMessage)
    {
        var requestMethod = requestMessage.Method.Method;
        var requestBody = await GetRequestBodyAsync(requestMessage);
        var result = await InvokeAsync<string>("get", requestMessage.RequestUri!, requestMethod, requestBody);

        return result;
    }

    public async ValueTask RemoveAsync(HttpRequestMessage requestMessage)
    {
        var requestMethod = requestMessage.Method.Method;
        var requestBody = await GetRequestBodyAsync(requestMessage);

        await InvokeVoidAsync("remove", requestMessage.RequestUri!, requestMethod, requestBody);
    }

    public async ValueTask RemoveAllAsync()
    {
        await InvokeVoidAsync("removeAll");
    }
    private static async ValueTask<string> GetRequestBodyAsync(HttpRequestMessage requestMessage)
    {
        var requestBody = string.Empty;
        if (requestMessage.Content is not null)
        {
            requestBody = await requestMessage.Content.ReadAsStringAsync();
        }

        return requestBody;
    }

    private async Task InitializeCacheAsync()
    {
        // last version cached is stored in appVersion
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "appVersion");

        // get the last version cached
        var result = await InternalGetAsync(requestMessage);
        string version = Constants.GetVersion();
        if (!result.Equals(version))
        {
            // running newer version now, clear cache, and update version in cache
            await RemoveAllAsync();
            var requestBody = await GetRequestBodyAsync(requestMessage);
            await InvokeVoidAsync(
                "put",
                requestMessage.RequestUri!,
                requestMessage.Method.Method,
                requestBody,
                version);
        }
        //
        CurrentCacheVersion = version;
    }
}
