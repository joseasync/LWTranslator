using LW.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace LW.Client.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
    {
        private bool _authenticated = false;
        private readonly HttpClient _httpClient;
        private readonly ClaimsPrincipal Unauthenticated;
        private readonly JsonSerializerOptions JsonSerializerOptions;
          

        public CustomAuthenticationStateProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("lwapi");
            Unauthenticated = new (new ClaimsIdentity());
            JsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // default to not authenticated
            _authenticated = false;
            var user = Unauthenticated;

            try
            {
                var userResponse = await _httpClient.GetAsync("manage/info");

                userResponse.EnsureSuccessStatusCode();

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, JsonSerializerOptions);

                if (userInfo != null)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email)
                    };

                    claims.AddRange(
                      userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                          .Select(c => new Claim(c.Key, c.Value)));

                    var id = new ClaimsIdentity(claims, nameof(CustomAuthenticationStateProvider));

                    user = new ClaimsPrincipal(id);

                    _authenticated = true;

                }
            }
            catch (Exception ex) { }

            return new AuthenticationState(user);
        }

        public async Task<FormResult> RegisterAsync(string email, string password)
        {
            string[] defaultDetail = ["Unknown error prevented registration from succeeding."];

            try
            {
                var result = await _httpClient.PostAsJsonAsync("register",
                      new { email, password });

                if (result.IsSuccessStatusCode)
                {
                    return new FormResult { Succeeded = true };
                }

                var details = await result.Content.ReadAsStringAsync();
                var problemDetails = JsonDocument.Parse(details);
                var errors = new List<string>();
                var errorList = problemDetails.RootElement.GetProperty("errors");

                foreach (var errorEntry in errorList.EnumerateObject())
                {
                    if (errorEntry.Value.ValueKind == JsonValueKind.String)
                    {
                        errors.Add(errorEntry.Value.GetString()!);
                    }
                    else if (errorEntry.Value.ValueKind == JsonValueKind.Array)
                    {
                        errors.AddRange(
                            errorEntry.Value.EnumerateArray().Select(
                                e => e.GetString() ?? string.Empty)
                            .Where(e => !string.IsNullOrEmpty(e)));
                    }
                }
                return new FormResult
                {
                    Succeeded = false,
                    ErrorList = problemDetails == null ? defaultDetail : [.. errors]
                };
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async Task<FormResult> LoginAsync(string email, string password)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync(
                    "login?useCookies=true", new
                    {
                        email,
                        password
                    });

                if (result.IsSuccessStatusCode)
                {
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return new FormResult { Succeeded = true };
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return new FormResult
            {
                Succeeded = false,
                ErrorList = ["Invalid email and/or password."]
            };
        }

        public async Task LogoutAsync()
        {
            const string Empty = "{}";
            var emptyContent = new StringContent(Empty, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync("account/Logout", emptyContent);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        }
        public async Task<bool> CheckAuthenticatedAsync()
        {
            await GetAuthenticationStateAsync();
            return _authenticated;
        }

    }
}
