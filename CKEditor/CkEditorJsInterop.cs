using Microsoft.JSInterop;

namespace CKEditor
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class CkEditorJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
        private Guid _id;


        public CkEditorJsInterop(IJSRuntime jsRuntime)
        {
            _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/CKEditor/ckeditor.js").AsTask());
        }

        public async ValueTask InitAsync(Guid id, DotNetObjectReference<InputCkEditor> reference)
        {
            _id = id;
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setup", new object[] { id, reference });
        }



        public async ValueTask DisposeAsync()
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                try
                {
                    await module.InvokeVoidAsync("destroy", _id);
                    await module.DisposeAsync();
                }
                catch (TaskCanceledException)
                {
                }
                catch (JSDisconnectedException)
                {
                }
            }
        }

        public async Task Update(string value)
        {
            if (_moduleTask.IsValueCreated)
            {
                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("update", new object[] { _id, value });
            }
        }
    }
}