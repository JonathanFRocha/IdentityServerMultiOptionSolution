using IdentityService.Models;
using IdentityService.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace IdentityService.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {

        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> Forecasts { get; set; }

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task OnGetAsync()
        {
            Forecasts = await InvokeEndpoint<List<WeatherForecastDTO>>("OurWebApi", "WeatherForecast");
        }

        private async Task<T> InvokeEndpoint<T>(string clientName, string url)
        {
            JwtToken token = null;

            var strTokenObj = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrWhiteSpace(strTokenObj))
            {
                token = await Authenticate();

            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);
            }

            if (
                token == null ||
                string.IsNullOrWhiteSpace(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate();
            }


            var httpClient = _httpClientFactory.CreateClient(clientName);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await httpClient.GetFromJsonAsync<T>(url);
        }

        private async Task<JwtToken> Authenticate()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebApi");
            var resp = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "Admin", Password = "admin" });

            resp.EnsureSuccessStatusCode();

            var strJwt = await resp.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }
    }
}
