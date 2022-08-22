using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using WebApplicationIdentityWithDb.Data.Account;
using WebApplicationIdentityWithDb.Services.interfaces;
using WebApplicationIdentityWithDb.Settings;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;
        private readonly IOptions<Secrets> setting;
        private readonly SignInManager<User> signInManager;

        [BindProperty]
        public EmailMFA EmailMFA { get; set; }

        public LoginTwoFactorModel(UserManager<User> userManager, IEmailService emailService, IOptions<Secrets> setting, SignInManager<User> signInManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.EmailMFA = new EmailMFA();
        }

        public async Task OnGetAsync(string email, bool rememberMe)
        {
            var user = await userManager.FindByEmailAsync(email);
            EmailMFA.SecurityCode = String.Empty;
            EmailMFA.RememberMe = rememberMe;
            // Generate the code
            var securityCode = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
            //Send to User
            await emailService.Send(
                setting.Value.EmailSender,
                email,
                "My Web's OTP",
                $"Please use this code as the OTP: {securityCode}"
                );
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorSignInAsync("Email", this.EmailMFA.SecurityCode, this.EmailMFA.RememberMe, false);

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
    public class EmailMFA
    {
        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }
    }

}
