using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Services;
using FormularPortal.Core;
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
            var form = await formService.GetAsync(FormId, dbController);


            if (form is not null)
            {
                foreach (var element in form.GetElements())
                {
                    if (element is FormTableElement formTableElement)
                    {
                        formTableElement.NewRow();
                    }
                }

                Input = form;
            }
        }
    }
}