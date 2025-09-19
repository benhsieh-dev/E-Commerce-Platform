using System.Text;
using Newtonsoft.Json;

namespace ApiGateway.Services
{
    public interface IHttpService
    {
        Task<T?> GetAsync<T>(string endpoint, string? token = null);
        Task<T?> PostAsync<T>(string endpoint, object data, string? token = null);
        Task<T?> PutAsync<T>(string endpoint, object data, string? token = null);
        Task<bool> DeleteAsync(string endpoint, string? token = null);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }

                _logger.LogWarning("GET request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making GET request to {Endpoint}", endpoint);
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = content
                };

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }

                _logger.LogWarning("POST request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", endpoint);
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
                {
                    Content = content
                };

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                }

                _logger.LogWarning("PUT request to {Endpoint} failed with status {StatusCode}", endpoint, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making PUT request to {Endpoint}", endpoint);
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, string? token = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making DELETE request to {Endpoint}", endpoint);
                return false;
            }
        }
    }
}