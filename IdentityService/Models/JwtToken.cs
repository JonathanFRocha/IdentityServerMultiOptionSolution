using Newtonsoft.Json;

namespace IdentityService.Models
{
    public class JwtToken
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
