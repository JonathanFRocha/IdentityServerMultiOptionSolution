using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApplicationIdentityWithDb.Data.Account;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LoginModel(SignInManager<User> signInManager)
        {
            this._signInManager = signInManager;
        }

        [BindProperty]
        public CredentialViewModel Credential { get; set; }

        [BindProperty]
        public IEnumerable<AuthenticationScheme> ExternalLoginProviders { get; set; }
        public async Task OnGetAsync()
        {
            ExternalLoginProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _signInManager.PasswordSignInAsync(
                Credential.Email,
                Credential.Password,
                Credential.RememberMe,
                false
                );

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.RequiresTwoFactor)
                {

                    return RedirectToPage("/Account/LoginTwoFactor", new
                    {
                        Email = this.Credential.Email,
                        RememberMe = this.Credential.RememberMe
                    });
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login2FA", "Failed to Login");
                }
                return Page();
            }
        }

        public IActionResult OnPostLoginExternally(string provider)
        {
            var props = _signInManager.ConfigureExternalAuthenticationProperties(provider, null);
            props.RedirectUri = Url.Action("ExternalLoginCallback", "Account");

            return Challenge(props, provider);
        }
    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
