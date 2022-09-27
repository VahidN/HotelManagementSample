using BlazorServer.Models;

namespace BlazorWasm.Client.Services;

public interface IClientAuthenticationService
{
    Task<AuthenticationResponseDto?> LoginAsync(AuthenticationDto userFromAuthentication);
    Task LogoutAsync();
    Task<RegistrationResponseDto?> RegisterUserAsync(UserRequestDto userForRegistration);
}