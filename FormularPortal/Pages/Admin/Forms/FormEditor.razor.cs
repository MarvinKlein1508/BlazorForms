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
        public Form Input { get; set; } = new Form();
        public FormElement? SelectedFormElement { get; set; }
        protected override Task OnParametersSetAsync()
        {
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
                Input.RemoveEmptyRows();
            }
            else if (dragDropServiceElements.ActiveItem is not null)
            {
                dragDropServiceElements.Items.Remove(dragDropServiceElements.ActiveItem);
            }
        }


        public Task OnColumnDroppedAsync(FormColumn column)
        {
            Input.RemoveEmptyRows();
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





        public Task OnToolbarElementDragStartAsync(FormElement element)
        {
            dragDropServiceElements.ActiveItem = (FormElement)element.Clone();
            dragDropServiceElements.Items = new List<FormElement>();
            StateHasChanged();
            return Task.CompletedTask;
        }
    }
}