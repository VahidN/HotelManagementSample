using BlazorServer.Common;
using BlazorServer.Entities;
using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasm.WebApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenFactoryService _tokenFactoryService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ITokenFactoryService tokenFactoryService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _tokenFactoryService = tokenFactoryService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] UserRequestDto userRequestDto)
    {
        if (userRequestDto is null)
        {
            return BadRequest("userRequestDto is null");
        }

        var user = new ApplicationUser
                   {
                       UserName = userRequestDto.Email,
                       Email = userRequestDto.Email,
                       Name = userRequestDto.Name,
                       PhoneNumber = userRequestDto.PhoneNo,
                       EmailConfirmed = true,
                   };
        var result = await _userManager.CreateAsync(user, userRequestDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new RegistrationResponseDto { Errors = errors, IsRegistrationSuccessful = false });
        }

        var roleResult = await _userManager.AddToRoleAsync(user, ConstantRoles.Customer);
        if (!roleResult.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new RegistrationResponseDto { Errors = errors, IsRegistrationSuccessful = false });
        }

        return Ok(new RegistrationResponseDto { IsRegistrationSuccessful = true });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] AuthenticationDto authenticationDto)
    {
        if (authenticationDto is null)
        {
            return BadRequest("authenticationDto is null");
        }

        var result = await _signInManager.PasswordSignInAsync(authenticationDto.UserName, authenticationDto.Password,
                                                              false, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new AuthenticationResponseDto
                                {
                                    IsAuthSuccessful = false,
                                    ErrorMessage = "Invalid Authentication",
                                });
        }

        var user = await _userManager.FindByNameAsync(authenticationDto.UserName);
        if (user == null)
        {
            return Unauthorized(new AuthenticationResponseDto
                                {
                                    IsAuthSuccessful = false,
                                    ErrorMessage = "Invalid Authentication",
                                });
        }

        var token = await _tokenFactoryService.CreateJwtTokensAsync(user);
        return Ok(new AuthenticationResponseDto
                  {
                      IsAuthSuccessful = true,
                      Token = token,
                      UserDto = new UserDto
                                {
                                    Name = user.Name ?? user.Email,
                                    Id = user.Id,
                                    Email = user.Email,
                                    PhoneNo = user.PhoneNumber,
                                },
                  });
    }
}