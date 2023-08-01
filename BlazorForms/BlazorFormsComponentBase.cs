using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace BlazorForms
{
    public abstract class BlazorFormsComponentBase : ComponentBase
    {
        [Inject]
        protected IStringLocalizer<App> AppLocalizer { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;


        protected string GetBooleanText(bool condition) => condition ? AppLocalizer["YES"] : AppLocalizer["NO"];
    }
}
