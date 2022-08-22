using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using WebApplicationIdentityWithDb.Data.Account;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public AuthenticatorMFA AuthenticatorMFA { get; set; }

        public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
        {
            AuthenticatorMFA = new AuthenticatorMFA();
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public async void OnGet(bool rememberMe)
        {
            AuthenticatorMFA.RememberMe = rememberMe;
            AuthenticatorMFA.SecurityCode = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
                AuthenticatorMFA.SecurityCode,
                AuthenticatorMFA.RememberMe,
                false
                );

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You are locked out.");
                }
                else
                {
                    ModelState.AddModelError("Login", "Failed to Login");
                }
                return Page();
            }
        }
    }

    public class AuthenticatorMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; }
    }
}
