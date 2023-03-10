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
using FormPortal.Core.Services;
using FormularPortal.Components.Modals;
using DatabaseControllerProvider;
using Blazor.Pagination;
using Blazor.BootstrapTabs;
using FormularPortal.Core;
using BlazorTooltips;
using vNext.BlazorComponents.FluentValidation;
using FormPortal.Core.Constants;
using FormPortal.Core.Interfaces;
using FormPortal.Core.Models;
using FormPortal.Core.Filters;
using FormPortal.Core.Models.FormElements;
using BlazorContextMenu;
using CKEditor;
using BlazorDownloadFile;
using FormPortal.Core.Pdf;

namespace FormularPortal.Components
{
    public partial class EntryList
    {
        [Parameter]
        public FormEntryFilter Filter { get; set; } = new();
        public int TotalItems { get; set; }
        public List<EntryListItem> Data { get; set; } = new();

        [Parameter, EditorRequired]
        public string NavUrl { get; set; } = string.Empty;
        public List<EntryListItem> DownloadingList { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
        }

        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            if (navigateToPage1)
            {
                navigationManager.NavigateTo(NavUrl);
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            TotalItems = await formEntryService.GetTotalAsync(Filter, dbController);
            Data = await formEntryService.GetAsync(Filter, dbController);
        }

        private async Task DownloadAsync(EntryListItem item)
        {
            DownloadingList.Add(item);
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);
            var entry = await formEntryService.GetAsync(item.EntryId, dbController);
            if (entry is not null)
            {
                var report = await ReportFormEntry.CreateAsync(entry);

                var data = report.GetBytes();

                string filename = item.Name;

                if (string.IsNullOrWhiteSpace(filename))
                {
                    filename = $"{item.FormName}_{item.EntryId}";
                }

                await downloadService.DownloadFile($"{filename}.pdf", data, "application/pdf");
            }
            DownloadingList.Remove(item);
        }
    }
}