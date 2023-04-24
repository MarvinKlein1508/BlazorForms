using FormPortal.Core.Services;
using DatabaseControllerProvider;
using Blazor.Pagination;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;

namespace FormPortal.Pages.Admin
{
    public partial class FormStatusManagement
    {
        public FormStatusFilter Filter { get; set; } = new();
     
        protected override Task NewAsync()
        {
            var newStatus = new FormStatus();

            foreach (var culture in AppdatenService.SupportedCultures)
            {
                newStatus.Description.Add(new FormStatusDescription
                {
                    Code = culture.TwoLetterISOLanguageName
                });
            }

            Input = newStatus;

            return Task.CompletedTask;
        }

    }
}