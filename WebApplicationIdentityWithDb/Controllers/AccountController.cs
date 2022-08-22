using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplicationIdentityWithDb.Data.Account;

namespace WebApplicationIdentityWithDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var loginInfo = await signInManager.GetExternalLoginInfoAsync();
            var emailClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            var userClaim = loginInfo.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            if(emailClaim != null && userClaim != null)
            {
                var user = new User { Email = emailClaim.Value, UserName = userClaim.Value };
                await signInManager.SignInAsync(user, false);
            }

            return RedirectToPage("/Index");
        }
    }
}
