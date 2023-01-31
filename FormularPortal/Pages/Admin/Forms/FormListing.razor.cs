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
using FormularPortal.Core.Filters;
using Blazor.Pagination;

namespace FormularPortal.Pages.Admin.Forms
{
    public partial class FormListing : IHasPagination
    {
        public FormFilter Filter { get; set; } = new FormFilter();

        public List<Form> Items { get; set; } = new();
        public int Page { get; set; }
        public int TotalItems { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
        }
        public async Task LoadAsync(bool navigateToPage1 = false)
        {
            Filter.PageNumber = Page;

        }

    }
}