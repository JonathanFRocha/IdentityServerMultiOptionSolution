using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApplicationIdentityWithDb.Data.Account;

namespace WebApplicationIdentityWithDb.Pages.Account
{
    [Authorize]
    public class PageProfileModel : PageModel
    {
        
        private readonly UserManager<User> userManager;

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }

        [BindProperty]
        public string? SuccessMessage { get; set; }

        public PageProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            UserProfile = new UserProfileViewModel();
            SuccessMessage = string.Empty;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

            UserProfile.Email = user.Email;
            UserProfile.Department = departmentClaim?.Value;
            UserProfile.Position = positionClaim?.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return Page();
            }


            try
            {
                var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
                await userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));
                await userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
            }catch
            {
                ModelState.AddModelError("UserProfile", "Error Occured when saving user profile");
            };

            this.SuccessMessage = "The User profile has been saved successfully";
            return Page();

        }

        private async  Task<(Data.Account.User, Claim, Claim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var claims = await userManager.GetClaimsAsync(user);
            var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
            var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

            return (user, departmentClaim, positionClaim);
        }
    }

    public class UserProfileViewModel
    {
        public string Email { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Position { get; set; }

    }
}
