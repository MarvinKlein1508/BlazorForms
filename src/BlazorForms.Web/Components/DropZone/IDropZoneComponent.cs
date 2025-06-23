using Microsoft.AspNetCore.Components;

namespace BlazorForms.Web.Components.DropZone;

public interface IDropZoneComponent<TItem>
{
    TItem? Value { get; }

    string? Id { get; }

    RenderFragment? Component { get; }
}
