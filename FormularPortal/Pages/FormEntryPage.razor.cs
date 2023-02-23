using DatabaseControllerProvider;
using FormPortal.Core.Models;
using FormPortal.Core.Models.FormElements;
using FormPortal.Core.Services;
using FormPortal.Core.Validators;
using FormPortal.Core.Validators.Admin;
using FormularPortal.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FormularPortal.Pages
{
    public partial class FormEntryPage
    {
        [Parameter]
        public int FormId { get; set; }
        public FormEntry? Input { get; set; }
        private EditForm? _form;
        protected override async Task OnParametersSetAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            var form = await formService.GetAsync(FormId, dbController);


            if (form is not null)
            {
                Input = new FormEntry(form);
            }
        }

        private async Task SubmitAsync()
        {
            if(_form is null || _form.EditContext is null)
            {
                return;
            }
            //Input.Form.Name = "TEST";
            _form.EditContext.Validate();
        }
    }
}