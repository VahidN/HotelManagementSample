namespace BlazorServer.Models;

public class RegistrationResponseDto
{
    public bool IsRegistrationSuccessful { get; set; }

    public IEnumerable<string> Errors { get; set; } = new List<string>();
}