using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationIdentityWithDb.Data.Account;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public string Message { get; set; }

        public ConfirmEmailModel(UserManager<User> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if(user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    Message = "Email Address is sucessfully confirmed, now you can try to login.";
                    return Page();
                }
            }

            Message = "Failed to validate email.";
            return Page();
        }
    }
}
