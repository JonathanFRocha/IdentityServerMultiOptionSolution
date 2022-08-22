using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApplicationIdentityWithDb.Data.Account;
using WebApplicationIdentityWithDb.Services.interfaces;
using WebApplicationIdentityWithDb.Settings;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IOptions<Secrets> smtpSetting;

        public RegisterModel(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService, IOptions<Secrets> smtpSetting)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.emailService = emailService;
            this.smtpSetting = smtpSetting;
        }

        [BindProperty]
        public RegisterViewModel registerViewModel { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validating Email address

            // Create the user
            var user = new User
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email,
                //Department = registerViewModel.Department,
                //Position = registerViewModel.Position
            };

            var claimDerpatment = new Claim("Department", registerViewModel.Department);
            var claimPosition = new Claim("Position", registerViewModel.Position);

            var result = await userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {

                await userManager.AddClaimAsync(user, claimDerpatment);
                await userManager.AddClaimAsync(user, claimPosition);

                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new {userId = user.Id, token = confirmationToken });

                await emailService.Send(
                    smtpSetting.Value.EmailSender,
                    user.Email,
                    "Please confirm your email",
                    $"Please click on this link to confirm your email address: {confirmationLink}"
                    );
                
                return RedirectToPage("/Account/Login");
            }else
            {
                
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }

        public class RegisterViewModel
        {
            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required]
            [DataType(dataType:DataType.Password)]
            public string Password { get; set; }
            [Required]
            public string Department { get; set; }
            [Required]
            public string Position { get; set; }
        }
    }
}
