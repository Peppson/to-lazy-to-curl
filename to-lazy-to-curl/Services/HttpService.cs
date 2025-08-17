using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using to_lazy_to_curl.Components;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Services;

public static class HttpService
{
    public static ICSharpCode.AvalonEdit.TextEditor? JsonResponseBody { get; set; }
    private const long _connectionTimeout = Config.ConnectionTimeout;
    private const int _messageDuration = Config.MessageDuration;

    public static async Task SubmitRequestAsync(string url, string json)
    {
        if (!CanSubmitRequest(url, json))
            return;

        _ = UiService.ShowMessageAsync("Sending...", "PrimaryText", 10000);

        var httpAction = States.SelectedHttpAction;
        await TrySendRequestAsync(url, json, httpAction);
    }

    private static async Task TrySendRequestAsync(string url, string json, HttpAction httpAction)
    {
        var errorMsg = GetRequestFailedMessage();

        try
        {
            await SendRequestAsync(url, json, httpAction);
        }
        catch (HttpRequestException ex)
        {
            _ = UiService.ShowMessageAsync(errorMsg, "Failure", _messageDuration);
            UiService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
        }
        catch (TaskCanceledException)
        {
            _ = UiService.ShowMessageAsync(GetTimeoutMessage(), "Failure", _messageDuration);
        }
        catch (Exception ex)
        {
            _ = UiService.ShowMessageAsync(errorMsg, "Failure", _messageDuration);
            UiService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
        }
    }

    private static async Task SendRequestAsync(string url, string json, HttpAction httpAction)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(_connectionTimeout)
        };
        var body = new StringContent(json, Encoding.UTF8, "application/json");

        // Send it!
        var response = await SendHttpRequestAsync(client, httpAction, url, body);
        ShowHttpResponseMessage(response);


        // todo update response ??
        if (JsonResponseBody != null)
        {
            var responseText = await response.Content.ReadAsStringAsync();
            JsonResponseBody.Text = responseText;
        }

    }

    private static async Task<HttpResponseMessage> SendHttpRequestAsync(HttpClient client, HttpAction action, string url, HttpContent? body = null)
    {
        return action switch
        {
            HttpAction.GET => await client.GetAsync(url),
            HttpAction.POST => await client.PostAsync(url, body),
            HttpAction.PUT => await client.PutAsync(url, body),
            HttpAction.PATCH => await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = body }),
            HttpAction.DELETE => await client.DeleteAsync(url),
            _ => throw new InvalidOperationException("No valid HTTP action selected.")
        };
    }

    private static (bool IsUrlValid, bool IsJsonValid) ValidateInputs(string url, string json)
    {
        bool isUrlValid = !string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute);
        bool isJsonValid = !string.IsNullOrWhiteSpace(json);

        return (isUrlValid, isJsonValid);
    }

    private static bool GetIsJsonRequired(HttpAction action)
    {
        return action switch
        {
            HttpAction.GET => false,
            HttpAction.POST => true,
            HttpAction.PUT => true,
            HttpAction.PATCH => true,
            HttpAction.DELETE => false,
            _ => false
        };
    }

    private static bool CanSubmitRequest(string url, string json)
    {
        var (IsUrlValid, IsJsonValid) = ValidateInputs(url, json);

        if (!IsUrlValid)
        {
            _ = UiService.ShowMessageAsync("Please enter a valid URL!", "Failure", _messageDuration);
            return false;
        }

        if (States.SelectedHttpAction == HttpAction.NONE)
        {
            _ = UiService.ShowMessageAsync("Please choose an HTTP action!", "Failure", _messageDuration);
            return false;
        }

        var isJsonRequired = GetIsJsonRequired(States.SelectedHttpAction);

        if (isJsonRequired && !IsJsonValid)
        {
            _ = UiService.ShowMessageAsync(GetJsonRequiredMessage(), "Failure", _messageDuration);
            return false;
        }

        // Let GET and DELETE pass through without Json body
        if ((isJsonRequired && IsJsonValid) ||
            States.SelectedHttpAction == HttpAction.GET ||
            States.SelectedHttpAction == HttpAction.DELETE)
        {
            return true;
        }

        return false; // Should never get here
    }

    public static void ShowHttpResponseMessage(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            _ = UiService.ShowMessageAsync(GetSuccessMessage(), "Success", _messageDuration);
        }
        else
        { 
            _ = UiService.ShowMessageAsync($"{(int)response.StatusCode}: {response.ReasonPhrase}", "Failure", _messageDuration);
        }
    }

    private static string GetJsonRequiredMessage()
    {
        var httpAction = States.SelectedHttpAction.ToString();
        return $"Provide a JSON body for the {httpAction} request!";
    }

    private static string GetRequestFailedMessage()
    {
        var httpAction = States.SelectedHttpAction.ToString();
        return $"Failed to send {httpAction} request";
    }

    private static string GetTimeoutMessage()
    {
        var httpAction = States.SelectedHttpAction.ToString();
        return $"{httpAction} request timed out after {_connectionTimeout} seconds!";
    }
    
    private static string GetSuccessMessage()
    {
        var httpAction = States.SelectedHttpAction.ToString();
        return $"{httpAction} request sent successfully!";
    }
}
