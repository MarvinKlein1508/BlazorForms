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
using System.Runtime.InteropServices;

namespace FormPortal.Components
{
    public partial class StatusChangeEditor
    {
        [Parameter, EditorRequired]
        public FormEntryStatusChange? Input { get; set; }
        [Parameter, EditorRequired]
        public List<FormStatus> Statuses { get; set; } = new();
        [Parameter, EditorRequired]
        public EventCallback OnCancel { get; set; }
        [Parameter, EditorRequired]
        public EventCallback OnSaved { get; set; }

        private async Task SaveAsync()
        {
            if(Input is null)
            {
                return;
            }

            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.ConnectionString);
            Input.DateAdded = DateTime.Now;

            if (Input.Id is 0)
            {
                await formEntryStatusChangeService.CreateAsync(Input, dbController);
            }
            else
            {
                await formEntryStatusChangeService.UpdateAsync(Input, dbController);
            }
        }
    }
}