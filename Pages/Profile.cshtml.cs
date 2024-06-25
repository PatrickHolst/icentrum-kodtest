using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace icentrum_kodtest.Pages
{
    public class ProfileModel : PageModel
    {
        public string? ErrorMessage { get; set; }
        public string? Image { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = await client.GetAsync("https://services2.i-centrum.se/recruitment/profile/avatar");
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        JObject jsonResponse = JObject.Parse(responseContent);
                        string Data = jsonResponse["data"].ToString();

                        Image = Data;

                        return Page();
                    }
                    else
                    {
                        ErrorMessage = $"Failed to retrieve profile avatar. Status code: {response.StatusCode}";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }
    }
}
