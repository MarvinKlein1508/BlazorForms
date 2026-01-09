using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CKEditor;

public partial class InputCkEditor : IAsyncDisposable
{
    private readonly Guid _guid = Guid.NewGuid();
    private readonly DotNetObjectReference<InputCkEditor> _reference;
    private IJSObjectReference? _jsModule;
    public const int MAX_HEIGHT = 400;

    public StringBuilder _sb = new();

    [Parameter]
    public bool ReadOnly { get; set; }

    public InputCkEditor()
    {
        _reference = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./Components/CKEditor/InputCkEditor.razor.js");
            await _jsModule.InvokeVoidAsync("setup", [_guid, _reference]);
            await _jsModule.InvokeVoidAsync("setReadonly", [_guid, ReadOnly]);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public async Task EditorDataChanged(string data, bool isCompleted)
    {
        if (_jsModule is null)
        {
            return;
        }

        _sb.Append(data);

        if (isCompleted)
        {
            string value = _sb.ToString();
            _sb.Clear();
            Value = value;
            await ValueChanged.InvokeAsync(Value);
            EditContext?.NotifyFieldChanged(FieldIdentifier);
        }
    }

    public async Task UpdateAsync(string data)
    {
        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("update", _guid, data);
        }
    }
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsModule is not null)
            {
                await _jsModule.InvokeVoidAsync("destroy", _guid);
                await _jsModule.DisposeAsync();
            }
        }
        catch (Exception ex) when (ex is JSDisconnectedException or
                                   OperationCanceledException)
        {
            // The JSRuntime side may routinely be gone already if the reason we're disposing is that
            // the client disconnected. This is not an error.
        }
    }
}
