using FormPortal.Core.Filters;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;

namespace FormPortal.Pages.Admin.Forms
{
    public partial class FormEntries
    {
        [Parameter]
        public int Page { get; set; }

        public FormEntryFilter Filter { get; set; } = new()
        {
            Limit = AppdatenService.PageLimit
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