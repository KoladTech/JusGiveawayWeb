using Microsoft.AspNetCore.Components;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace JusGiveawayWebApp.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _httpClient;
        private string webApiKey = "AIzaSyA-WcxOGCdd56pOAdLrYpsqTl4NnI7WLvw";
        private readonly string _firebaseBaseUrl;
        private string _authToken; // Authentication token (optional)

        public FirebaseService(HttpClient httpClient, string firebaseBaseUrl, string authToken = null)
        {
            _httpClient = httpClient;
            _firebaseBaseUrl = firebaseBaseUrl;
            _authToken = authToken;
        }
        public void SetAuthToken(string authToken)
        {
            _authToken = authToken;
        }
        public async Task<FirebaseAuthResponse?> SignUpUserAsync(string email, string password)
        {
            var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={webApiKey}"; // Use string interpolation

            var payload = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsJsonAsync(requestUri, payload);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();

                // Store the UID in IndexedDB or use it as needed
                Console.WriteLine("Signup successful!");
                return jsonResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Signup failed: {errorContent}");
                return null;
            }
        }

        public async Task<FirebaseAuthResponse?> SignInEmailPassword(string email, string password)
        {
            var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={webApiKey}";

            var requestBody = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<FirebaseAuthResponse>();

                //var o = await RefreshIdToken(jsonResponse.RefreshToken);

                // Process the response (e.g., store tokens, user info)
                Console.WriteLine("Sign in successful!");
                return jsonResponse;
            }
            else
            {
                Console.WriteLine("Sign in failed.");
                return null;
            }
        }

        public async Task SignOutAsync()
        {
            //var response = await _httpClient.PostAsync("https://identitytoolkit.googleapis.com/v1/accounts:signOut?key=[API_KEY]", null);

            var requestUri = $"https://identitytoolkit.googleapis.com/v1/accounts:signOut?key={webApiKey}";

            // Send a POST request to sign out the user
            try
            {
                var response = await _httpClient.PostAsync(requestUri, null);

                if (response.IsSuccessStatusCode)
                {
                    // Successfully signed out
                    // You can also clear any local storage or IndexedDB here if needed
                    Console.WriteLine("User signed out successfully.");
                }
                else
                {
                    // Handle error response
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error signing out: {errorContent}");
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }


        public async Task<string> RefreshIdToken(string refreshToken)
        {
            // Create the request body with the refresh token
            var requestBody = new
            {
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };

            // Firebase token refresh URL with your Firebase Web API Key
            string requestUri = $"https://securetoken.googleapis.com/v1/token?key={webApiKey}";

            // Create HTTP request with content
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // Make the POST request
            var response = await _httpClient.PostAsync(requestUri, content);

            // Parse the response if successful
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(jsonResponse);
                var newIdToken = jsonDocument.RootElement.GetProperty("id_token").GetString();
                return newIdToken; // Return the new ID token
            }

            // Handle error or return null if the refresh fails
            return null;
        }


        // Method to write data to Firebase
        public async Task<bool> WriteDataAsync<T>(string path, T data)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/{path}.json";
                if (!string.IsNullOrEmpty(_authToken))
                {
                    url += $"?auth={_authToken}";
                }

                var response = await _httpClient.PutAsJsonAsync(url, data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to Firebase: {ex.Message}");
                return false;
            }
        }

        // Method to read data from Firebase
        public async Task<T> ReadDataAsync<T>(string path)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/{path}.json";
                if (!string.IsNullOrEmpty(_authToken))
                {
                    url += $"?auth={_authToken}";
                }

                //var responsetest = await _httpClient.GetStringAsync(url);
                //Console.WriteLine(responsetest); // Log the raw JSON response for debugging

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }
                else
                {
                    Console.WriteLine($"Error reading from Firebase: {response.ReasonPhrase}");
                    return default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from Firebase: {ex.Message}");
                return default;
            }
        }

        // Method to delete data from Firebase
        public async Task<bool> DeleteDataAsync(string path)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/{path}.json";
                if (!string.IsNullOrEmpty(_authToken))
                {
                    url += $"?auth={_authToken}";
                }

                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting from Firebase: {ex.Message}");
                return false;
            }
        }

        public async Task<HttpResponseMessage?> PollFirebaseForLeftoverGiveawayFunds(string lastETag)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/Giveaways/A/LeftoverGiveawayFunds.json";
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (lastETag != null)
                {
                    request.Headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue(lastETag));
                }
                request.Headers.Add("X-Firebase-ETag", "true");

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Data has changed, process the new data
                    return response;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
                {
                    // Data has not changed, no need to update
                    Console.WriteLine("No data changes detected.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
            return null;
        }
    }

    public class FirebaseAuthResponse
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expiresIn")]
        public string ExpiresIn { get; set; }

        [JsonPropertyName("localId")] // This is the UID
        public string LocalId { get; set; }

        [JsonPropertyName("registered")]
        public bool Registered { get; set; }
    }

}
