using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasm.Client.Pages.Authentication;

public partial class RedirectToLogin
{
    private AuthenticationState? AuthState { set; get; }
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;

    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { set; get; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is null)
        {
            throw new InvalidOperationException("AuthenticationState is null");
        }

        AuthState = await AuthenticationState;
        if (!IsAuthenticated(AuthState))
        {
            var returnUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.NavigateTo(string.IsNullOrEmpty(returnUrl)
                                             ? "login"
                                             : $"login?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }
    }

    private static bool IsAuthenticated(AuthenticationState authState) =>
        authState?.User?.Identity is not null && authState.User.Identity.IsAuthenticated;
}