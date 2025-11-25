using BlazorForms.Domain.Common;
using BlazorForms.Domain.Filters;
using BlazorForms.Domain.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace BlazorForms.Web.Components.ComponentBases;

public abstract class ListPageBase<TModel, TService, TFilter> : BlazorFormsPageBase
     where TModel : class, new()
     where TService : IFilterOperations<TModel, TFilter>
     where TFilter : PageFilterBase, new()
{
    protected EditForm? _form;
    protected bool _isLoading;

    [Parameter] public TFilter Filter { get; set; } = new();
    [Inject] protected TService Service { get; set; } = default!;
    [SupplyParameterFromQuery] public int Page { get; set; } = 1;

    public PagedResponse<TModel>? Data { get; set; }

    protected bool UseFilterCaching { get; set; }

    protected Uri CurrentUri => NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
    protected override async Task OnParametersSetAsync()
    {
        if (Page <= 0)
        {
            Page = 1;
        }

        if (UseFilterCaching)
        {
            var localStorageResult = await LocalStorage.GetAsync<TFilter>(GetLocalStorageFilterKey());
            if (localStorageResult.Success)
            {
                if (localStorageResult.Value is not null)
                {
                    Filter = localStorageResult.Value;
                }
            }

            await ChangeCachedFilterAsync();
        }

        if (Filter is not null)
        {
            await LoadAsync();
        }
    }

    protected virtual Task ChangeCachedFilterAsync()
    {
        return Task.CompletedTask;
    }
    protected virtual string GetLocalStorageFilterKey()
    {
        return $"{typeof(TFilter).FullName}.Filter";
    }
    protected virtual async Task LoadAsync(bool navigateToPage1 = false)
    {
        _isLoading = true;
        await Task.Yield();
        if (navigateToPage1)
        {
            var parameters = GetQueryParameter();
            parameters.Remove("page");
            parameters.Add("page", "1");

            NavigationManager.NavigateTo($"{CurrentUri.AbsolutePath}?{BuildQueryParameters(parameters)}");
        }

        if (UseFilterCaching)
        {
            Filter.PageNumber = 1; // Always use page 1 as default
            await LocalStorage.SetAsync(GetLocalStorageFilterKey(), Filter);
        }

        Filter.PageNumber = Page < 1 ? 1 : Page;

        using var connection = await DbFactory.CreateConnectionAsync();
        Data = await Service.GetAsync(Filter, connection);
        _isLoading = false;
    }
    protected virtual Task OnPageChangedAsync(int pageNumber)
    {
        var parameters = GetQueryParameter();
        parameters.Remove("page");
        parameters.Add("page", pageNumber.ToString());

        NavigationManager.NavigateTo($"{CurrentUri.AbsolutePath}?{BuildQueryParameters(parameters)}");

        return Task.CompletedTask;
    }

    protected Dictionary<string, StringValues> GetQueryParameter() => QueryHelpers.ParseQuery(CurrentUri.Query);
    protected string BuildQueryParameters(Dictionary<string, StringValues> parameters)
    {
        if (parameters.Count == 0)
        {
            return string.Empty;
        }

        var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value!)}"));
        return queryString;
    }
}
