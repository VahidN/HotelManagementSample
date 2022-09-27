using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorServer.App.Areas.Identity.Pages.Account.Manage;

public class ShowRecoveryCodesModel : PageModel
{
    [TempData] public IList<string> RecoveryCodes { get; set; } = new List<string>();

    [TempData] public string? StatusMessage { get; set; }

    public IActionResult OnGet()
    {
        if (RecoveryCodes == null || RecoveryCodes.Count == 0)
        {
            return RedirectToPage("./TwoFactorAuthentication");
        }

        return Page();
    }
}