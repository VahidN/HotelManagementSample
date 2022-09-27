using System.Security.Claims;
using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorWasm.Client.Utils;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasm.Client.Services;

public class ClientAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<ClientAuthenticationStateProvider> _logger;

    public ClientAuthenticationStateProvider(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        ILogger<ClientAuthenticationStateProvider> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>(ConstantKeys.LocalToken);
        return token == null ? GetEmptyAuthenticationState() : await GetCurrentAuthenticationStateAsync(token);
    }

    public async Task NotifyUserLoggedInAsync(string token)
    {
        var authState = Task.FromResult(await GetCurrentAuthenticationStateAsync(token));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(GetEmptyAuthenticationState());
        NotifyAuthenticationStateChanged(authState);
    }

    private async Task<AuthenticationState> GetCurrentAuthenticationStateAsync(string token)
    {
        var jwtInfo = JwtParser.ParseClaimsFromJwt(token);
        if (jwtInfo.IsExpired)
        {
            _logger.LogWarning("The JWT was expired (@ {ExpirationDateUtc}) UTC and will be removed.",
                               jwtInfo.ExpirationDateUtc);
            await _localStorage.RemoveItemAsync(ConstantKeys.LocalToken);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return GetEmptyAuthenticationState();
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwtInfo.Claims, "jwtAuthType")));
    }

    private static AuthenticationState GetEmptyAuthenticationState() => new(new ClaimsPrincipal(new ClaimsIdentity()));
}