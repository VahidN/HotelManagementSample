using BlazorServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasm.WebApi.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "Editor")]
public class MyProtectedEditorsApiController : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(new ProtectedEditorsApiDto
           {
               Id = 1,
               Title = "Hello from My Protected Editors Controller!",
               Username = User.Identity?.Name,
           });
}