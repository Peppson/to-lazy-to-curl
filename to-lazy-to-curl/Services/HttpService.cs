using Newtonsoft.Json;

namespace to_lazy_to_curl.Services;

static public class HttpService
{
    private const long _connectionTimeout = 5; // s
    
    /* private static async Task<(bool Success, string? ErrorMsg)> SendPostRequestAsync(string url, string json)
    {
        const string genericError = "Error sending POST request!";
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(_connectionTimeout) };
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (false, "404: Endpoint not found!");
            }

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var reason = response.ReasonPhrase;
                ShowMessageBox($"{genericError} \n\nStatus: {statusCode} \nReason: {reason}\n ");
                return (false, genericError);
            }

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            ShowMessageBox($"{genericError}\n\n{ex.Message}");
            return (false, $"{genericError}");
        }
        catch (TaskCanceledException)
        {
            return (false, $"POST request timed out after {_connectionTimeout} seconds!");
        }
        catch (Exception ex)
        {
            ShowMessageBox($"{genericError}\n\n {ex.Message}");
            return (false, $"{genericError}");
        }
    } */

    /* private static FormState ValidateInputs(string url, string json)
    {
        var isUrlEmpty = string.IsNullOrWhiteSpace(url);
        var isJsonEmpty = string.IsNullOrWhiteSpace(json);

        isUrlEmpty = isUrlEmpty || !Uri.IsWellFormedUriString(url, UriKind.Absolute);

        if (isUrlEmpty && isJsonEmpty) return FormState.BothEmpty;
        if (isUrlEmpty) return FormState.UrlEmpty;
        if (isJsonEmpty) return FormState.JsonEmpty;

        return FormState.Filled;
    } */
}
