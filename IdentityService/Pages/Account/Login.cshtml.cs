using IdentityService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityService.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential credential { get; set; }
        public void OnGet()
        {
            credential = new Credential() { UserName = "Admin" };
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid) return Page();
            if (credential.UserName == "Admin" && credential.Password == "admin")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com"),
                    new Claim("Department", "HR"),
                    new Claim("Admin", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2021-05-01")
                };

                var identity = new ClaimsIdentity(claims, Constants.COOKIE_NAME);
                var claimsPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties { IsPersistent = credential.RememberMe };

                await HttpContext.SignInAsync(Constants.COOKIE_NAME, claimsPrincipal, authProperties);

                return RedirectToPage("/index");
            };
            return Page();
        }
    }
}
