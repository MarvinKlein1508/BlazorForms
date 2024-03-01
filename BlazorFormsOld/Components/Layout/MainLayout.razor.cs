using BlazorForms.Core.Constants;
using BlazorForms.Core.Models;
using BlazorForms.Core.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace BlazorForms.Components.Layout
{
    public partial class MainLayout
    {


        private const string JAVASCRIPT_FILE = "./_content/BlazorForms.Components.Layout/MainLayout.razor.js";
        private string? _version;
        private bool _mobile;
        private string? _prevUri;
        
        private bool _menuChecked = true;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

    

        protected override void OnInitialized()
        {
            var versionAttribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (versionAttribute != null)
            {
                var version = versionAttribute.InformationalVersion;
                var plusIndex = version.IndexOf('+');
                if (plusIndex >= 0 && plusIndex + 9 < version.Length)
                {
                    _version = version[..(plusIndex + 9)];
                }
                else
                {
                    _version = version;
                }
            }

            _prevUri = NavigationManager.Uri;
            NavigationManager.LocationChanged += LocationChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", JAVASCRIPT_FILE);
                _mobile = await jsModule.InvokeAsync<bool>("isDevice");
                await jsModule.DisposeAsync();
            }
        }

    
    

        private void HandleChecked()
        {
            _menuChecked = !_menuChecked;
        }

        private void LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (!e.IsNavigationIntercepted && new Uri(_prevUri!).AbsolutePath != new Uri(e.Location).AbsolutePath)
            {
                _prevUri = e.Location;
                if (_mobile && _menuChecked == true)
                {
                    _menuChecked = false;
                    StateHasChanged();
                }
            }
        }
    }
}