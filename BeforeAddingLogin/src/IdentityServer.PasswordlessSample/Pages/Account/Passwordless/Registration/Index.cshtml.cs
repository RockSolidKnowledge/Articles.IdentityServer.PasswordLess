using System.ComponentModel.DataAnnotations;
using IdentityServer.PasswordlessSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido;

namespace IdentityServer.PasswordlessSample.Pages.Account.Passwordless.Registration;

public class IndexModel
    (IFidoAuthentication fidoAuthentication, UserManager<ApplicationUser> userManager) : PageModel
{
    [Required] [BindProperty] public string Email { get; set; }

    [Required] [BindProperty] public string DeviceName { get; set; }

    [BindProperty(SupportsGet = true)] public String ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (await userManager.FindByNameAsync(Email) != null)
        {
            return BadRequest("A user with that username already exists.");
        }

        var challenge = await fidoAuthentication.InitiateRegistration(Email, DeviceName);

        var dto = challenge.ToBase64Dto();

        return RedirectToPage("Register", new
        {
            challenge.UserId,
            dto.Base64UserHandle,
            dto.Base64Challenge,
            dto.DeviceFriendlyName,
            dto.Base64ExcludedKeyIds,
            dto.RelyingPartyId,
            ReturnUrl
        });
    }
}