using Microsoft.AspNetCore.Components;

namespace BlazorForms.Components.Modals
{
    public class ModalBase : ComponentBase
    {
        [Parameter] public string Title { get; set; } = String.Empty;
        [Parameter] public EventCallback OnClosed { get; set; }

        protected string ModalSizeClass => ModalSize switch
        {
            ModalSize.Default => "modal-dialog",
            ModalSize.SM => "modal-dialog modal-sm",
            ModalSize.LG => "modal-dialog modal-lg",
            ModalSize.XL => "modal-dialog modal-xl",
            ModalSize.XXL => "modal-dialog modal-xxl",
            _ => "modal-dialog"
        };

        [Parameter] public ModalSize ModalSize { get; set; } = ModalSize.Default;
        /// <summary>
        /// Gibt an, ob das Modal innerhalb eines weiteren Modals geöffnet wird. Wenn ja, dann erhält das Modal noch zusätzlich die CSS Klasse
        /// .double-overlay
        /// </summary>
        [Parameter] public bool DoubleOverlay { get; set; } = false;
        [Parameter] public string CloseBtnClass { get; set; } = "btn-danger";
    }
}
