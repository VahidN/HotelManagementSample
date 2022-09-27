namespace BlazorServer.Models;

public class AuthenticationDto
{
    [Required(ErrorMessage = "UserName is required")]
    [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$",
                          ErrorMessage = "Invalid email address")]
    public string UserName { get; set; } = default!;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
}