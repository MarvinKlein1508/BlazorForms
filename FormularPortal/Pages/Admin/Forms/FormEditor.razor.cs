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
using FormularPortal.Components;
using Plk.Blazor.DragDrop;
using FormularPortal.Core.Models;

namespace FormularPortal.Pages.Admin.Forms
{
    public partial class FormEditor
    {
        public List<FormRow> Rows { get; set; } = new();
        public FormElement? SelectedFormElement { get; set; }
        protected override Task OnParametersSetAsync()
        {
            Rows.Add(new FormRow(1));
            return base.OnParametersSetAsync();
        }
        public void DropDelete()
        {
            if (dragDropServiceRows.ActiveItem is not null)
            {
                dragDropServiceRows.Items.Remove(dragDropServiceRows.ActiveItem);
            }
            else if (dragDropServiceColumns.ActiveItem is not null)
            {
                dragDropServiceColumns.Items.Remove(dragDropServiceColumns.ActiveItem);
                CheckForEmptyRows();
            }
            else if (dragDropServiceElements.ActiveItem is not null)
            {
                dragDropServiceElements.Items.Remove(dragDropServiceElements.ActiveItem);
            }
        }


        public Task OnColumnDroppedAsync(FormColumn column)
        {
            CheckForEmptyRows();
            return Task.CompletedTask;
        }

        public void StartDragColumnFromToolbar()
        {
            dragDropServiceColumns.ActiveItem = new FormColumn();
            dragDropServiceColumns.Items = new List<FormColumn>();
            StateHasChanged();
        }

        public void StartDragRowFromToolbar()
        {
            dragDropServiceRows.ActiveItem = new FormRow(1);
            dragDropServiceRows.Items = new List<FormRow>();
            StateHasChanged();
        }

        private void CheckForEmptyRows()
        {
            var list = Rows.Where(x => !x.Columns.Any()).ToList();
            foreach (var item in list)
            {
                Rows.Remove(item);
            }
        }



        public Task OnToolbarElementDragStartAsync(FormElement element)
        {
            dragDropServiceElements.ActiveItem = (FormElement)element.Clone();
            dragDropServiceElements.Items = new List<FormElement>();
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}