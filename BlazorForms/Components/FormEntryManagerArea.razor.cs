using BlazorForms.Core.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components
{
    public partial class FormEntryManagerArea
    {
        [Parameter, EditorRequired]
        public FormEntry? Entry { get; set; } 
    }
}