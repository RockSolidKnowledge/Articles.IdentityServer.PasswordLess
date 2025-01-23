using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rsk.AspNetCore.Fido;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.PasswordlessSample.Pages.Account.Passwordless.Login;

public class IndexModel(IFidoAuthentication fidoAuthentication) : PageModel
{
    [BindProperty] [EmailAddress] public string EmailAddress { get; set; }

    [BindProperty(SupportsGet = true)] public String ReturnUrl { get; set; }


    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        try
        {
            var challenge = await fidoAuthentication.InitiateAuthentication(EmailAddress);
            var dto = challenge.ToBase64Dto();

            return RedirectToPage("Login", new
            {
                dto.UserId,
                dto.Base64Challenge,
                dto.RelyingPartyId,
                dto.Base64KeyIds,
                ReturnUrl
            });
        }
        catch (PublicKeyCredentialException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}