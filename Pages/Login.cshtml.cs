using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace icentrum_kodtest.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestBody = new
                    {
                        username = Username,
                        password = Password
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://services2.i-centrum.se/recruitment/auth", jsonContent);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                        if (tokenResponse?.Token != null)
                        {
                            HttpContext.Session.SetString("AccessToken", tokenResponse.Token);
                            return RedirectToPage("/Profile");
                        }
                        else
                        {
                            ErrorMessage = $"Failed to retrieve access token. Response: {responseContent}";
                            return Page();
                        }
                    }
                    else
                    {
                        ErrorMessage = $"Invalid login attempt. Response: {responseContent}";
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

        public class TokenResponse
        {
            [JsonProperty("token")]
            public string? Token { get; set; }
        }
    }
}
