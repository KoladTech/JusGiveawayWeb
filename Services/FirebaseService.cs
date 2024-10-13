using Microsoft.AspNetCore.Components;
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
