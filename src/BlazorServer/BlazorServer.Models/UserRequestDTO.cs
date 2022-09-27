namespace BlazorServer.Models;

public class UserRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$",
                          ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = default!;

    public string? PhoneNo { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password is not matched")]
    public string ConfirmPassword { get; set; } = default!;
}