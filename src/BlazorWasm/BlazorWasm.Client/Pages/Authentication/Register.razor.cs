using BlazorServer.Models;
using BlazorWasm.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorWasm.Client.Pages.Authentication;

public partial class Register
{
    private UserRequestDto UserForRegistration { get; } = new();
    private IEnumerable<string>? Errors { set; get; }
    private bool IsProcessing { set; get; }
    private bool ShowRegistrationErrors { set; get; }

    [Inject] public IClientAuthenticationService AuthenticationService { set; get; } = default!;
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;

    private async Task RegisterUser()
    {
        ShowRegistrationErrors = false;
        IsProcessing = true;
        var result = await AuthenticationService.RegisterUserAsync(UserForRegistration);
        if (result?.IsRegistrationSuccessful == true)
        {
            IsProcessing = false;
            NavigationManager.NavigateTo("/login");
        }
        else
        {
            IsProcessing = false;
            Errors = result?.Errors;
            ShowRegistrationErrors = true;
        }
    }
}