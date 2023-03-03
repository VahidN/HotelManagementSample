using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorWasm.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWasm.Client.Services;

public class ClientHttpInterceptorService : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;

    public ClientHttpInterceptorService(
        NavigationManager navigationManager,
        ILocalStorageService localStorage,
        IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                 CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // How to add a JWT to all of the requests
        var token = await _localStorage.GetItemAsync<string>(ConstantKeys.LocalToken, cancellationToken);
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await _jsRuntime.ToastrError($"Failed to call `{request.RequestUri}`. StatusCode: {response.StatusCode}.");

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    _navigationManager.NavigateTo("/404");
                    break;
                case HttpStatusCode.Forbidden: // 403
                case HttpStatusCode.Unauthorized: // 401
                    _navigationManager.NavigateTo("/unauthorized");
                    break;
                default:
                    _navigationManager.NavigateTo("/500");
                    break;
            }
        }

        return response;
    }
}