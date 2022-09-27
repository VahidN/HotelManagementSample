using Microsoft.AspNetCore.Identity;

namespace BlazorServer.Entities;

public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }
}