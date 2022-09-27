using BlazorServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorServer.App.Areas.Identity.Pages.Account.Manage;

public class PersonalDataModel : PageModel
{
    private readonly ILogger<PersonalDataModel> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public PersonalDataModel(
        UserManager<ApplicationUser> userManager,
        ILogger<PersonalDataModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        return Page();
    }
}