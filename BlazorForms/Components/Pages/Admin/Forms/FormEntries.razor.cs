using BlazorForms.Core;
using BlazorForms.Core.Filters;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components.Pages.Admin.Forms
{
    public partial class FormEntries
    {
        [Parameter]
        public int Page { get; set; }

        public FormEntryFilter Filter { get; set; } = new()
        {
            Limit = Storage.PageLimit
        };

        protected override Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            Filter.PageNumber = Page;

            return base.OnParametersSetAsync();
        }
    }
}