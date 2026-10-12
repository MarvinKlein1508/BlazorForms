using BlazorForms.Domain;
using BlazorForms.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorForms.Web.Components;

public abstract class ListPageBase<TModel, TRepository, TFilter> : BlazorFormsPageBase
    where TModel : class, new()
    where TRepository : IFilterOperations<TModel, TFilter>
    where TFilter : PagedFilter, new()
{
    protected bool _isLoading;
    protected bool _isInitialized;

    [Inject] protected TRepository Repository { get; set; } = default!;

    [Parameter] public TFilter Filter { get; set; } = new();

    protected IQueryable<TModel> Items { get; set; } = Enumerable.Empty<TModel>().AsQueryable();

    protected PaginationState _pagination { get; set; } = new PaginationState() { ItemsPerPage = 30 };

    [Parameter, SupplyParameterFromQuery]
    public int Page { get; set; } = 1;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Page <= 0)
        {
            Page = 1;
        }

        _isInitialized = true;

        if (Filter is not null)
        {
            var startIndex = (Page - 1) * Filter.PageSize;
            await RefreshItemsAsync(new GridItemsProviderRequest<TModel>()
            {
                StartIndex = startIndex
            });
        }
    }

    protected async Task RefreshItemsAsync(GridItemsProviderRequest<TModel> request)
    {
        if (!_isInitialized)
        {
            return;
        }

        _isLoading = true;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(BlazorFormConstants.FLUENTUI_SEARCH_DELAY_IN_MS); // Fix for missing values

        Filter.Page = (request.StartIndex / Filter.PageSize) + 1;

        if (Page != Filter.Page)
        {
            SetPage(Filter.Page);
        }

        using var connection = await DbFactory.CreateConnectionAsync();
        var data = await Repository.GetAsync(Filter, connection);
        if (data is null)
        {
            await _pagination.SetCurrentPageIndexAsync(0);
            await _pagination.SetTotalItemCountAsync(0);
            Items = Enumerable.Empty<TModel>().AsQueryable();
        }
        else
        {
            await _pagination.SetCurrentPageIndexAsync(Filter.Page - 1);
            await _pagination.SetTotalItemCountAsync(data.Total);
            Items = data.Items.AsQueryable();
        }

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    protected void SetPage(int page)
    {
        var newUrl = NavigationManager.GetUriWithQueryParameter("page", page);
        NavigationManager.NavigateTo(newUrl, replace: true);
    }
}
