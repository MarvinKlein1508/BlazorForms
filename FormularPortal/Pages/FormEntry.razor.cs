using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using FormularPortal;
using FormularPortal.Components;
using Plk.Blazor.DragDrop;
using FormularPortal.Core.Services;
using FormularPortal.Core.Models;
using FormularPortal.Components.Modals;
using DatabaseControllerProvider;
using Blazor.Pagination;
using Blazor.BootstrapTabs;
using FormularPortal.Core;
using BlazorTooltips;
using vNext.BlazorComponents.FluentValidation;

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