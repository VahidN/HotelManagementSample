using System.Web;
using BlazorServer.Models;
using BlazorWasm.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorWasm.Client.Pages.Authentication;

public partial class Login
{
    private AuthenticationDto UserForAuthentication { get; } = new();
    private string? Errors { set; get; }
    private bool IsProcessing { set; get; }
    private string? ReturnUrl { set; get; }
    private bool ShowAuthenticationErrors { set; get; }

    [Inject] public IClientAuthenticationService AuthenticationService { set; get; } = default!;
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;

    private async Task LoginUser()
    {
        ShowAuthenticationErrors = false;
        IsProcessing = true;
        var result = await AuthenticationService.LoginAsync(UserForAuthentication);
        if (result?.IsAuthSuccessful == true)
        {
            IsProcessing = false;
            var absoluteUri = new Uri(NavigationManager.Uri);
            var queryParam = HttpUtility.ParseQueryString(absoluteUri.Query);
            ReturnUrl = queryParam["returnUrl"];
            NavigationManager.NavigateTo(string.IsNullOrEmpty(ReturnUrl) ? "/" : $"/{ReturnUrl}");
        }
        else
        {
            IsProcessing = false;
            Errors = result?.ErrorMessage;
            ShowAuthenticationErrors = true;
        }
    }
}