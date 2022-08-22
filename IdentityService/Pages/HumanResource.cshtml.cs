using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages
{
    [Authorize(Policy = "MustBelongToHr")]
    public class HumanResourceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
