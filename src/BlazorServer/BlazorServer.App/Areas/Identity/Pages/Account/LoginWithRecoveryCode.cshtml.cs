using BlazorServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorServer.App.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginWithRecoveryCodeModel : PageModel
{
    private readonly ILogger<LoginWithRecoveryCodeModel> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginWithRecoveryCodeModel(SignInManager<ApplicationUser> signInManager,
                                      ILogger<LoginWithRecoveryCodeModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty] public InputModel Input { get; set; } = default!;

    public string? ReturnUrl { get; set; }

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException("Unable to load two-factor authentication user.");
        }

        ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            throw new InvalidOperationException("Unable to load two-factor authentication user.");
        }

        var recoveryCode = Input.RecoveryCode.Replace(" ", string.Empty, StringComparison.Ordinal);

        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

        if (result.Succeeded)
        {
            _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
            return LocalRedirect(returnUrl ?? Url.Content("~/"));
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
            return RedirectToPage("./Lockout");
        }

        _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
        ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
        return Page();
    }

    public class InputModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; } = default!;
    }
}