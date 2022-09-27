using BlazorWasm.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorWasm.Client.Pages.Authentication;

public partial class Logout
{
    [Inject] public IClientAuthenticationService AuthenticationService { set; get; } = default!;
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await AuthenticationService.LogoutAsync();
        NavigationManager.NavigateTo("/");
    }
}