using IdentityServer.PasswordlessSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido.Dtos;
using Rsk.AspNetCore.Fido;

namespace IdentityServer.PasswordlessSample.Pages.Account.Passwordless.Registration;

public class RegisterModel
    (IFidoAuthentication fidoAuthentication, UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty(SupportsGet = true)] public Base64FidoRegistrationChallenge Challenge { get; set; }

    [BindProperty(SupportsGet = true)] public String ReturnUrl { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost([FromBody] Base64FidoRegistrationResponse response)
    {
        var result = await fidoAuthentication.CompleteRegistration(response.ToFidoResponse());

        if (result.IsError)
        {
            return BadRequest(result.ErrorDescription);
        }

        ApplicationUser user = new()
        {
            UserName = result.UserId,
            Email = result.UserId
        };
        IdentityResult userCreationResult = await userManager.CreateAsync(user);
        if (userCreationResult.Succeeded)
            return new EmptyResult();
        else
        {
            return BadRequest(String.Join(',', userCreationResult.Errors.Select(e => e.Description)));
        }
    }
}