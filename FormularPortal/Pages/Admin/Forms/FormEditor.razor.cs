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
using DatabaseControllerProvider;
using FormularPortal.Core.Services;
using FormularPortal.Core;
using Microsoft.AspNetCore.Mvc.Formatters;
using FormularPortal.Core.Validators.Admin;
using FluentValidation;

namespace FormularPortal.Pages.Admin.Forms
{
    public partial class FormEditor
    {
        [Parameter]
        public int FormId { get; set; }
        public Form? Input { get; set; }

        public Form StartCopy { get; set; } = new();
        public FormElement? SelectedFormElement { get; set; }

        public bool EditFormProperties { get; set; }
        protected override async Task OnParametersSetAsync()
        {

            if (FormId > 0)
            {
                // This Task will take some time depending on the size of the form.
                // To not block the UI we run it in a different Task.
                await Task.Run(LoadEditModeAsync);

            }
            else
            {
                Input = new Form();
                Input.Rows.Add(new FormRow(1));
            }
        }

        public async Task LoadEditModeAsync()
        {
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);

            Form? form = await formService.GetAsync(FormId, dbController);

            if (form is not null)
            {
                Input = form.DeepCopyByExpressionTree();
                StartCopy = form.DeepCopyByExpressionTree();
            }
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
                Input?.RemoveEmptyRows();
            }
            else if (dragDropServiceElements.ActiveItem is not null)
            {
                dragDropServiceElements.Items.Remove(dragDropServiceElements.ActiveItem);
            }
        }
        public Task OnColumnDroppedAsync(FormColumn column)
        {
            Input?.RemoveEmptyRows();
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
            var newElement = element.DeepCopyByExpressionTree();
            newElement.GenerateGuid();
            dragDropServiceElements.ActiveItem = newElement;
            dragDropServiceElements.Items = new List<FormElement>();
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            if (Input is null)
            {
                return;
            }

            foreach (var element in Input.GetElements())
            {

                IValidator validator = element.GetValidator();
                IValidationContext context = new ValidationContext<FormElement>(element);
                if (validator.Validate(context).IsValid)
                {
                    await jsRuntime.ShowToastAsync(ToastType.success, $"{element} is valid");
                }
                else
                {
                    await jsRuntime.ShowToastAsync(ToastType.error, $"{element} is not valid");
                }

            }

            return;
            using IDbController dbController = dbProviderService.GetDbController(AppdatenService.DbProvider, AppdatenService.ConnectionString);

            //await dbController.StartTransactionAsync();

            try
            {
                if (Input.FormId is 0)
                {
                    await formService.CreateAsync(Input, dbController);

                }
                else
                {
                    await formService.UpdateAsync(Input, StartCopy, dbController);

                }

                //await dbController.CommitChangesAsync();
            }
            catch (Exception)
            {
                //await dbController.RollbackChangesAsync();
                throw;
            }

            if (FormId is 0)
            {
                navigationManager.NavigateTo($"/Admin/FormEditor/{Input.FormId}");
            }
            else
            {
                await OnParametersSetAsync();
            }

            await jsRuntime.ShowToastAsync(ToastType.success, "Form has been saved successfully.");

        }
    }
}