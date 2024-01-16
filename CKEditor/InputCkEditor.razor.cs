using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor
{
    public partial class InputCkEditor : IAsyncDisposable
    {
        private string _originalValue = String.Empty;
        [Parameter] public string Value { get; set; } = String.Empty;
#nullable disable
        private CkEditorJsInterop _interop;
#nullable enable
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private readonly Guid _id;

        public InputCkEditor()
        {
            _id = Guid.NewGuid();
        }


        [JSInvokable]
        public async Task EditorDataChanged(string? data)
        {
            data ??= String.Empty;
            Value = data;
            _originalValue = Value;
            await ValueChanged.InvokeAsync(data);

        }
        protected override async Task OnInitializedAsync()
        {
            _interop = new CkEditorJsInterop(jsRuntime);
            await _interop.InitAsync(_id, DotNetObjectReference.Create(this));
        }

        public async ValueTask DisposeAsync()
        {
            if (_interop is not null)
            {
                await _interop.DisposeAsync();
            }

        }

        protected override async Task OnParametersSetAsync()
        {
            if (!String.Equals(_originalValue, Value, StringComparison.OrdinalIgnoreCase))
            {
                await _interop.Update(Value);
            }
        }

    }
}