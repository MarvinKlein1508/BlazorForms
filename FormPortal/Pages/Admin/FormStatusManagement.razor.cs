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
using FormPortal;
using FormPortal.Components;
using Plk.Blazor.DragDrop;
using FormPortal.Core.Services;
using FormPortal.Components.Modals;
using DatabaseControllerProvider;
using Blazor.Pagination;
using Blazor.BootstrapTabs;
using FormPortal.Core;
using BlazorTooltips;
using vNext.BlazorComponents.FluentValidation;
using FormPortal.Core.Constants;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;
using FormPortal.Core.Models.FormElements;
using CKEditor;
using BlazorDownloadFile;
using BlazorInputTags;
using Toolbelt.Blazor.HotKeys2;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using FormPortal.Core.Settings;

namespace FormPortal.Pages.Admin
{
    public partial class FormStatusManagement : IHasPagination
    {
        public FormStatusFilter Filter { get; set; } = new();
        public int Page { get; set; }
        public int TotalItems { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Page < 1)
            {
                Page = 1;
            }

            await LoadAsync();
        }

        protected override async Task SaveAsync()
        {
            await base.SaveAsync();
            await LoadAsync();
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            Filter.PageNumber = navigateToPage1 ? 1 : Page;
            using IDbController dbController = DbProviderService.GetDbController(AppdatenService.ConnectionString);
            TotalItems = await Service.GetTotalAsync(Filter, dbController);
            Data = await Service.GetAsync(Filter, dbController);
        }
    }
}