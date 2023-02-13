using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Services;
using Microsoft.AspNetCore.Components;

namespace FormularPortal.Pages
{
    public partial class FormEntry
    {
        [Parameter]
        public int FormId { get; set; }
        public Form? Input { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            Input = await formService.GetAsync(FormId, dbController);
        }
    }
}