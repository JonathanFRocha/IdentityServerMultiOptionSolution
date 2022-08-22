using Microsoft.AspNetCore.Identity;

namespace WebApplicationIdentityWithDb.Data.Account
{
    public class User:IdentityUser
    {
        public string Department { get; set; } = String.Empty;
        public string Position { get; set; } = String.Empty;
    }
}
