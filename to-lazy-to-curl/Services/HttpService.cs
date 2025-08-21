using System.Net.Http;
using System.Text;
using Serilog;
using System.Text.Json;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Settings;

namespace to_lazy_to_curl.Services;

public static class HttpService
{
    public static async Task SubmitRequestAsync(string url, string body)
    {
        var httpAction = AppState.SelectedHttpAction;
        if (!CanSubmitRequest(url, httpAction)) return;

        _ = MessageService.ShowTextMessageAsync("Sending...", "PrimaryText", 10000);

        await TrySendRequestAsync(url, body, httpAction);
    }

    private static async Task TrySendRequestAsync(string url, string body, HttpAction httpAction)
    {
        var errorMsg = GetRequestFailedMessage();

        try
        {
            await SendRequestAsync(url, body, httpAction);
        }
        catch (HttpRequestException ex)
        {
            _ = MessageService.ShowTextMessageAsync(errorMsg, "Failure", Config.StatusMessageDuration);
            MessageService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
        }
        catch (TaskCanceledException)
        {
            _ = MessageService.ShowTextMessageAsync(GetTimeoutMessage(), "Failure", Config.StatusMessageDuration);
        }
        catch (Exception ex)
        {
            _ = MessageService.ShowTextMessageAsync(errorMsg, "Failure", Config.StatusMessageDuration);
            MessageService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
        }
    }

    private static async Task SendRequestAsync(string url, string body, HttpAction httpAction)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(Config.HttpConnectionTimeout)
        };

        var headers = GetHeaders(); // todo headers?
        var response = await SendHttpRequestAsync(url, body, client, httpAction, headers);

        ShowHttpResponseMessage(response);
        await SetHttpResponseText(response);
    }

    private static async Task<HttpResponseMessage> SendHttpRequestAsync(string url, string body, HttpClient client, HttpAction httpAction, Dictionary<string, string>? headers = null)
    {
        var contentType = AppState.PayloadEditorSyntax ?? "application/json";

        var request = new HttpRequestMessage
        {
            Method = GetHttpAction(httpAction),
            RequestUri = new Uri(url),
            Content = new StringContent(body, Encoding.UTF8, GetContentType(contentType))
        };

        // No headers
        if (headers == null)
            return await client.SendAsync(request);

        // Headers
        foreach (var header in headers)
        {   
            if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }
        
        return await client.SendAsync(request);
    }
    
    private static HttpMethod GetHttpAction(HttpAction action)
    {
        return action switch
        {
            HttpAction.GET => HttpMethod.Get,
            HttpAction.POST => HttpMethod.Post,
            HttpAction.PUT => HttpMethod.Put,
            HttpAction.DELETE => HttpMethod.Delete,
            HttpAction.PATCH => HttpMethod.Patch,
            _ => throw new InvalidOperationException("No valid HTTP method selected.")
        };
    }
    
    private static string GetContentType(string contentTypeHeader)
    {
        return contentTypeHeader switch
        {
            SyntaxHighlighting.Json => "application/json",
            SyntaxHighlighting.Html => "text/html",
            SyntaxHighlighting.Xml => "application/xml",
            _ => "text/plain"
        };
    }

    private static Dictionary<string, string>? GetHeaders(bool headers = false)
    {
        if (!headers) return null;

        return new Dictionary<string, string>
        {
            { "Authorization", "Bearer XYZ" }, // todo dynamic
            { "Cache-Control", "no-cache" },
        };
    }

    private static async Task SetHttpResponseText(HttpResponseMessage? response)
    {
        if (response == null || AppState.JsonInput == null)
            return;

        var contentType = response.Content?.Headers.ContentType?.MediaType;

        // JSON
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("Response is JSON");
            AppState.JsonInput.ResponseEditor.Text = await GetResponseAsJsonAsync(response); 
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Json;
            return;
        }

        // HTML
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("html", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("Response is HTML");
            AppState.JsonInput.ResponseEditor.Text = await response.Content!.ReadAsStringAsync();
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Html;
            return;
        }

        // XML
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("Response is XML");
            AppState.JsonInput.ResponseEditor.Text = await response.Content!.ReadAsStringAsync();
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Xml;
            return;
        }

        // Fallback
        Log.Debug("Response fallback: Plain Text");
        AppState.ResponseEditorSyntax = SyntaxHighlighting.PlainText;
        AppState.JsonInput.ResponseEditor.Text = await response.Content!.ReadAsStringAsync();
    }

    private static async Task<string> GetResponseAsJsonAsync(HttpResponseMessage response)
    {
        var responseText = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseText))
            return "";
        
        using var jsonDoc = JsonDocument.Parse(responseText);
        
        return JsonSerializer.Serialize(
            jsonDoc.RootElement,
            new JsonSerializerOptions { WriteIndented = true });
    }

    private static bool ValidateInput(string url)
    {
        return !string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }

    private static bool CanSubmitRequest(string url, HttpAction httpAction)
    {
        var IsUrlValid = ValidateInput(url);

        if (!IsUrlValid)
        {
            _ = MessageService.ShowTextMessageAsync("Please enter a valid URL!", "Failure", Config.StatusMessageDuration);
            return false;
        }

        if (httpAction == HttpAction.NONE)
        {
            _ = MessageService.ShowTextMessageAsync("Please choose an HTTP action!", "Failure", Config.StatusMessageDuration);
            return false;
        }

        return true;
    }

    private static void ShowHttpResponseMessage(HttpResponseMessage response)
    {
        string color = response.IsSuccessStatusCode ? "Success" : "Failure";
        _ = MessageService.ShowTextMessageAsync($"{(int)response.StatusCode}: {response.ReasonPhrase}", color, Config.StatusMessageDuration);
    }

    private static string GetBodyRequiredMessage()
    {
        var httpAction = AppState.SelectedHttpAction.ToString();
        return $"Provide a payload for the {httpAction} request!";
    }

    private static string GetRequestFailedMessage()
    {
        var httpAction = AppState.SelectedHttpAction.ToString();
        return $"Failed to send {httpAction} request";
    }

    private static string GetTimeoutMessage()
    {
        var httpAction = AppState.SelectedHttpAction.ToString();
        return $"{httpAction} request timed out after {Config.HttpConnectionTimeout} seconds!";
    }
    
    private static string GetSuccessMessage()
    {
        var httpAction = AppState.SelectedHttpAction.ToString();
        return $"{httpAction} request sent successfully!";
    }
}
