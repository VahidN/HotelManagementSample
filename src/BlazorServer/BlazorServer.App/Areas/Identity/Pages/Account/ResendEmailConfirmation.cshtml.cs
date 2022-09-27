using System.Text;
using System.Text.Encodings.Web;
using BlazorServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorServer.App.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ResendEmailConfirmationModel : PageModel
{
    private readonly IEmailSender _emailSender;
    private readonly UserManager<ApplicationUser> _userManager;

    public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    [BindProperty] public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
                                   "/Account/ConfirmEmail",
                                   null,
                                   new { userId, code },
                                   Request.Scheme);
        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            throw new InvalidOperationException("callbackUrl is null");
        }

        await _emailSender.SendEmailAsync(
                                          Input.Email,
                                          "Confirm your email",
                                          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
        return Page();
    }

    public class InputModel
    {
        [Required] [EmailAddress] public string Email { get; set; } = default!;
    }
}