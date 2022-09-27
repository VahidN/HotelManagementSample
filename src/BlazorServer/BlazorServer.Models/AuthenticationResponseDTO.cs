namespace BlazorServer.Models;

public class AuthenticationResponseDto
{
    public bool IsAuthSuccessful { get; set; }

    public string? ErrorMessage { get; set; }

    public string? Token { get; set; }

    public UserDto? UserDto { get; set; }
}