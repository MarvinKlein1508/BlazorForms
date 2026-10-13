using BlazorForms.Domain;
using BlazorForms.Infrastructure;
using BlazorForms.Infrastructure.Database;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorForms.Components;

public partial class FormComponentBase<TModel> : ComponentBase
{
    protected EditContext? _editContext;

    protected virtual bool AutoSaveEnabled => false;

    [Parameter, EditorRequired]
    public TModel Input { get; set; } = default!;

    [Parameter, EditorRequired]
    public EditMode EditMode { get; set; }

    [Parameter, EditorRequired]
    public EventCallback<TModel> OnValidSubmit { get; set; }

    [Parameter]
    public RenderFragment? ToolbarTemplate { get; set; }

    [CascadingParameter]
    public User? CurrentUser { get; set; }

    [Inject]
    public IDialogService DialogService { get; set; } = default!;

    [Inject]
    public IDbConnectionFactory DbFactory { get; set; } = default!;

    [Inject]
    public INotificationService NotificationService { get; set; } = default!;

    protected override Task OnInitializedAsync()
    {
        if (Input is null)
        {
            throw new InvalidOperationException("Input cannot be null.");
        }

        if (CurrentUser is null)
        {
            throw new InvalidOperationException("CurrentUser cannot be null.");
        }

        _editContext = new EditContext(Input);

        return base.OnInitializedAsync();
    }

    protected async Task SubmitAsync()
    {
        if (_editContext is null || Input is null)
        {
            return;
        }

        if (_editContext.Validate())
        {
            await OnValidSubmit.InvokeAsync(Input);
        }
    }
}

public abstract class FormComponentBase<TModel, TValidator> : FormComponentBase<TModel>
    where TValidator : AbstractValidator<TModel>, new()
{
    protected TValidator _validator = new TValidator();
}
