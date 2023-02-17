using Microsoft.JSInterop;
using System.Threading;

namespace FormularPortal
{
    public static class IJSRuntimeExtensions
    {
        /// <summary>
        /// Zeigt ein SweetAlert2 an.
        /// </summary>
        /// <param name="js"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="timer"></param>
        /// <returns></returns>
        public static ValueTask ShowSweetAlert2Async(this IJSRuntime js, string title, string message, SweetAlertMessageType type, int timer = 0)
        {
            return js.InvokeVoidAsync("blazorHelpers.showSweetAlert", type.ToString(), title, message, timer);
        }

        /// <summary>
        /// Zeigt eine Toastbenachrichtigung an.
        /// </summary>
        /// <param name="js"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static ValueTask ShowToastAsync(this IJSRuntime js, ToastType type, string message, int timeout = 5000)
        {
            return js.InvokeVoidAsync("blazorHelpers.showToast", type.ToString(), message, timeout);
        }

        public static ValueTask ScrollToFragment(this IJSRuntime js, string id, ScrollBehavior behavior = ScrollBehavior.auto)
        {
            return js.InvokeVoidAsync("blazorHelpers.scrollToFragment", id, behavior.ToString());
        }
    }

    public enum ToastType
    {
        warning, error, success
    }
    public enum SweetAlertMessageType
    {
        question, warning, error, success, info
    }

    public enum ScrollBehavior
    {
        auto,
        smooth
    }
}
