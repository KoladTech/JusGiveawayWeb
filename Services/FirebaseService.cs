using JusGiveawayWebApp.Helpers;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JusGiveawayWebApp.Services
{
    public class FirebaseService
    {
        private readonly HttpClient _httpClient;
        private string webApiKey = "AIzaSyA-WcxOGCdd56pOAdLrYpsqTl4NnI7WLvw";
        private readonly string _firebaseBaseUrl;
        private string _authToken; // Authentication token
        private string _refreshToken; // Authentication refresh token

        public FirebaseService(HttpClient httpClient, string firebaseBaseUrl, string authToken = null, string refreshToken = null)
        {
            _httpClient = httpClient;
            _firebaseBaseUrl = firebaseBaseUrl;
            _authToken = authToken;
            _refreshToken = refreshToken;
        }
        public async Task<bool> SetAuthTokenIfNotExpired(string authToken)
        {
            //it token is expired, sign out user
            if (await IsTokenExpired(authToken))
            {
                //try to refresh token
                //authToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjhkOWJlZmQzZWZmY2JiYzgyYzgzYWQwYzk3MmM4ZWE5NzhmNmYxMzciLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vanVzZ2l2ZWF3YXkiLCJhdWQiOiJqdXNnaXZlYXdheSIsImF1dGhfdGltZSI6MTcyODk4OTUzNiwidXNlcl9pZCI6ImdVc2NEUFdwcTBaN3dJVVlNU3U5VjZjSU5IdjEiLCJzdWIiOiJnVXNjRFBXcHEwWjd3SVVZTVN1OVY2Y0lOSHYxIiwiaWF0IjoxNzI4OTg5NTM2LCJleHAiOjE3Mjg5OTMxMzYsImVtYWlsIjoiaEBoLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJlbWFpbCI6WyJoQGguY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGFzc3dvcmQifX0.pfV5s7vKQ14jgzPM9FRQp46weyG9geo5ZLCCG8xBzUJBjBUnQDtdtdwAIJq1AIezCNpWa9Cfc4BmszUf-iMmFVrp8msubTXbEpHpawmxMJhiyup1-do1hn1k1VHSjIUKyCe5I0VYamoWSv_47ghlQjkvSjsmlr7lFnKWwzEUUlGo6HSYbwP1iwiaJidNdp4JvgSH8RHo_nh7ocLTgphB6diaM0JL7NsQZJwYkW4H81xTuwRfeFxIFiWzu9caUAa4RERNCI4Ybv4GqIYXKdfmpfwfSqslN8HoM2Cd6ga-tOJnD6Mx7rBB30wl3yfmRGqTTjxokf7PqS506ClMDBQs2g";

                string? refreshedToken = await RefreshIdToken(_refreshToken);
                if (refreshedToken == null)
                {
                    return true;
                }
                authToken = refreshedToken;
            }
            _authToken = authToken;
            return false;
        }

        public void SetRefreshToken(string refreshToken)
        {   
            _refreshToken = refreshToken;
        }

        public async Task<bool> IsTokenExpired(string idToken)
        {
            try
            {
                //test token - an old one that's expired
                //idToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjhkOWJlZmQzZWZmY2JiYzgyYzgzYWQwYzk3MmM4ZWE5NzhmNmYxMzciLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vanVzZ2l2ZWF3YXkiLCJhdWQiOiJqdXNnaXZlYXdheSIsImF1dGhfdGltZSI6MTcyODk4OTUzNiwidXNlcl9pZCI6ImdVc2NEUFdwcTBaN3dJVVlNU3U5VjZjSU5IdjEiLCJzdWIiOiJnVXNjRFBXcHEwWjd3SVVZTVN1OVY2Y0lOSHYxIiwiaWF0IjoxNzI4OTg5NTM2LCJleHAiOjE3Mjg5OTMxMzYsImVtYWlsIjoiaEBoLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiZmlyZWJhc2UiOnsiaWRlbnRpdGllcyI6eyJlbWFpbCI6WyJoQGguY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGFzc3dvcmQifX0.pfV5s7vKQ14jgzPM9FRQp46weyG9geo5ZLCCG8xBzUJBjBUnQDtdtdwAIJq1AIezCNpWa9Cfc4BmszUf-iMmFVrp8msubTXbEpHpawmxMJhiyup1-do1hn1k1VHSjIUKyCe5I0VYamoWSv_47ghlQjkvSjsmlr7lFnKWwzEUUlGo6HSYbwP1iwiaJidNdp4JvgSH8RHo_nh7ocLTgphB6diaM0JL7NsQZJwYkW4H81xTuwRfeFxIFiWzu9caUAa4RERNCI4Ybv4GqIYXKdfmpfwfSqslN8HoM2Cd6ga-tOJnD6Mx7rBB30wl3yfmRGqTTjxokf7PqS506ClMDBQs2g";
                
                var jwtHandler = new JwtSecurityTokenHandler();

                if (jwtHandler.CanReadToken(idToken))
                {
                    var jwtToken = jwtHandler.ReadJwtToken(idToken);
                    //var exp = jwtToken.Payload.Exp; //Exp is obsolete
                    var exp = jwtToken.Payload.Expiration; // Expiration time in seconds

                    // Convert the exp (Unix timestamp) to DateTime
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;

                    // Compare with current time
                    if (expirationTime <= DateTime.UtcNow)
                    {
                        return true; // Token is expired
                    }
                }
                else
                {
                    return true; //Invalid Token
                }
                return false; // Token is valid
            }
            catch (Exception ex)
            {
                await WriteErrorMessagesAsync($"Error checking expiration of authtoken : {ex.Message}", "FirebaseService - IsTokenExpired()", DateTime.Now.ToString(), needsAuthToken: false);
                Console.WriteLine($"Error checking expiration of authtoken : {ex.Message}");
                return true; 
            }
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
                await WriteErrorMessagesAsync($"Signup failed: {errorContent}", payload?.ToString() ?? "", DateTime.Now.ToString(), needsAuthToken: false);
                Console.WriteLine($"Signup failed: {errorContent}");
                return null;
            }
        }

        public async Task SendEmailVerification(string idToken)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={webApiKey}";
            var emailVerificationRequest = new
            {
                requestType = "VERIFY_EMAIL",
                idToken
            };

            var response = await _httpClient.PostAsJsonAsync(url, emailVerificationRequest);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Email verification sent.");
            }
            else
            {
                Console.WriteLine("Failed to send verification email.");
            }
        }

        public async Task<bool> IsEmailVerified(string idToken)
        {
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={webApiKey}";
            var request = new
            {
                idToken
            };

            var response = await _httpClient.PostAsJsonAsync(url, request);
            var result = await response.Content.ReadFromJsonAsync<LookupResponse>();

            return result.Users.FirstOrDefault()?.EmailVerified ?? false;
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
                //await WriteErrorMessagesAsync("Sign in failed. Possibly wrong credentials", content?.ToString() ?? "", DateTime.Now.ToString(), needsAuthToken: false);
                Console.WriteLine("Sign in failed. Possibly wrong credentials");
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


        public async Task<string?> RefreshIdToken(string refreshToken)
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
        public async Task<bool> WriteDataAsync<T>(string path, T data, bool needsAuthToken)
        {
            try
            {
                if (string.IsNullOrEmpty(_authToken) && needsAuthToken)
                {
                    return false;
                }

                var url = $"{_firebaseBaseUrl}/{path}.json";
                if (!string.IsNullOrEmpty(_authToken) && needsAuthToken)
                {
                    bool authTokenExpired = await SetAuthTokenIfNotExpired(_authToken);
                    if (authTokenExpired)
                    {
                        await WriteErrorMessagesAsync($"Error renewing authtoken for WRITING to firebaseDB", url, DateTime.Now.ToString(), needsAuthToken: false);
                        Console.WriteLine($"Error renewing authtoken for WRITING to firebaseDB");
                        return false;
                    }

                    url += $"?auth={_authToken}";
                }

                var response = await _httpClient.PutAsJsonAsync(url, data);
                var success = response.IsSuccessStatusCode;
                if (!success) {
                    await WriteErrorMessagesAsync($"Error writing to Firebase: {response.ReasonPhrase}", url + " - " + data?.ToString(), DateTime.Now.ToString(), needsAuthToken: false);
                }
                return success;
            }
            catch (Exception ex)
            {
                await WriteErrorMessagesAsync($"Error writing to Firebase: {ex.Message}", path + " - " + data?.ToString() ?? "", DateTime.Now.ToString(), needsAuthToken: false);
                Console.WriteLine($"Error writing to Firebase: {ex.Message}");
                return false;
            }
        }

        // Method to read data from Firebase
        public async Task<T> ReadDataAsync<T>(string path, bool needsAuthToken)
        {
            try
            {
                if (string.IsNullOrEmpty(_authToken) && needsAuthToken)
                {
                    return default;
                }

                var url = $"{_firebaseBaseUrl}/{path}.json";
                if (!string.IsNullOrEmpty(_authToken) && needsAuthToken)
                {
                    bool authTokenExpired = await SetAuthTokenIfNotExpired(_authToken);
                    if (authTokenExpired)
                    {
                        await WriteErrorMessagesAsync($"Error renewing authtoken for READING from firebaseDB", url, DateTime.Now.ToString(), needsAuthToken: false);
                        Console.WriteLine($"Error renewing authtoken for READING from firebaseDB");
                        return default;
                    }

                    url += $"?auth={_authToken}";
                }

                //var responsetest = await _httpClient.GetStringAsync(url);
                //Console.WriteLine(responsetest); // Log the raw JSON response for debugging

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(contentString) || contentString == "null")
                    {
                        Console.WriteLine("Key does not exist or value is null.");
                        return default;
                    }
                    return await response.Content.ReadFromJsonAsync<T>();
                }
                else
                {
                    await WriteErrorMessagesAsync($"Error reading from Firebase: {response.ReasonPhrase}", url, DateTime.Now.ToString(), needsAuthToken: false);
                    Console.WriteLine($"Error reading from Firebase: {response.ReasonPhrase}");
                    return default;
                }
            }
            catch (Exception ex)
            {
                await WriteErrorMessagesAsync($"Error reading from Firebase: {ex.Message}", path, DateTime.Now.ToString(), needsAuthToken: false);
                Console.WriteLine($"Error reading from Firebase: {ex.Message}");
                return default;
            }
        }

        // Method to write errorMessages to Firebase
        public async Task WriteErrorMessagesAsync(string errorMessage, string data, string errorTime, bool needsAuthToken)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/ErrorMessages.json";
                if (!string.IsNullOrEmpty(_authToken) && needsAuthToken)
                {
                    url += $"?auth={_authToken}";
                }

                var versionFromClient = await GetLocalVersionAsync();

                var errorMessageData = new { ErrorMessage = errorMessage , Data = data, Time = errorTime, JGVersion = versionFromClient?.version ?? "null"};
                
                string errorMessageJson = JsonSerializer.Serialize(errorMessageData);

                var response = await _httpClient.PostAsync(url, new StringContent(errorMessageJson));
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error writing errorMessage messages to Firebase: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing errorMessage messages to Firebase: {ex.Message}");
            }
        }

        // Method to delete data from Firebase
        public async Task<bool> DeleteDataAsync(string path, bool needsAuthToken)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/{path}.json";
                if (!string.IsNullOrEmpty(_authToken) && needsAuthToken)
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

        public async Task<HttpResponseMessage?> PollFirebaseForLeftoverGiveawayFunds(string lastETag, string currentGiveaway, bool needsAuthToken)
        {
            try
            {
                var url = $"{_firebaseBaseUrl}/Giveaways/{currentGiveaway}/LeftoverGiveawayFunds.json";

                if (!string.IsNullOrEmpty(_authToken) && needsAuthToken)
                {
                    bool authTokenExpired = await SetAuthTokenIfNotExpired(_authToken);
                    if (authTokenExpired)
                    {
                        await WriteErrorMessagesAsync($"Error renewing authtoken for PollFirebaseForLeftoverGiveawayFunds", url, DateTime.Now.ToString(), needsAuthToken: false);
                        Console.WriteLine($"Error renewing authtoken for PollFirebaseForLeftoverGiveawayFunds");
                        return null;
                    }

                    url += $"?auth={_authToken}";
                }

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
                await WriteErrorMessagesAsync($"Error polling data from firebase: {ex.Message}", "PollingFirebaseForLeftOverFunds", DateTime.Now.ToString(), needsAuthToken: false);
                Console.WriteLine($"Error polling data from firebase: {ex.Message}");
            }
            return null;
        }

        public async Task<VersionInfo?> GetLocalVersionAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "JusGiveawayWebApp.version.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;

                using (StreamReader reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<VersionInfo>(json);
                }
            }
        }
        public class VersionInfo
        {
            public string version { get; set; }
            public string releaseDate { get; set; }
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
    public class LookupResponse
    {
        public List<User> Users { get; set; }
    }

    public class User
    {
        public string Email { get; set; }
        public bool EmailVerified { get; set; }
    }

}
