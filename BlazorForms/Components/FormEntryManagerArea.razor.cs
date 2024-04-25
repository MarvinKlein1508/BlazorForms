using BlazorForms.Core.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components
{
    public partial class FormEntryManagerArea
    {
        [Parameter, EditorRequired]
        public FormEntry? Entry { get; set; }

        public FormEntryManagerContent Input { get; set; } = new();
        protected override Task OnParametersSetAsync()
        {

            if (Entry is not null)
            {

            }

            return base.OnParametersSetAsync();
        }
    }
}