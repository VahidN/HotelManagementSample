using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorServer.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasm.Client.Services;

public class ClientAuthenticationService : IClientAuthenticationService
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly HttpClient _client;
    private readonly ILocalStorageService _localStorage;

    public ClientAuthenticationService(
        HttpClient client,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _client = client;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<AuthenticationResponseDto?> LoginAsync(AuthenticationDto userFromAuthentication)
    {
        var response = await _client.PostAsJsonAsync("api/account/signin", userFromAuthentication);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthenticationResponseDto>(responseContent);

        if (result?.Token is null || !response.IsSuccessStatusCode)
        {
            return result;
        }

        var resultToken = result.Token;
        await _localStorage.SetItemAsync(ConstantKeys.LocalToken, resultToken);
        await _localStorage.SetItemAsync(ConstantKeys.LocalUserDetails, result.UserDto);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", resultToken);

        await ((ClientAuthenticationStateProvider)_authStateProvider).NotifyUserLoggedInAsync(resultToken);

        return new AuthenticationResponseDto { IsAuthSuccessful = true };
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(ConstantKeys.LocalToken);
        await _localStorage.RemoveItemAsync(ConstantKeys.LocalUserDetails);
        _client.DefaultRequestHeaders.Authorization = null;

        ((ClientAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<RegistrationResponseDto?> RegisterUserAsync(UserRequestDto userForRegistration)
    {
        var response = await _client.PostAsJsonAsync("api/account/signup", userForRegistration);
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RegistrationResponseDto>(responseContent);

        if (response.IsSuccessStatusCode)
        {
            return new RegistrationResponseDto { IsRegistrationSuccessful = true };
        }

        return result;
    }
}