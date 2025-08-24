using System.Net.Http;
using System.Text;
using Serilog;
using System.Text.Json;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.Settings;
using Newtonsoft.Json;

namespace to_lazy_to_curl.Services;

public static class HttpService
{
    private const int _httpTimeoutMs = (int)(Config.HttpConnectionTimeout * 1000);
    
    public static async Task SubmitRequestAsync(string url, string body)
    {
        var httpAction = AppState.SelectedHttpAction;

        if (!CanSubmitRequest(url, httpAction)) return;

        _ = MessageService.ShowTextMessageAsync(
            "Sending",
            "PrimaryText",
            StatusIcon.Send,
            _httpTimeoutMs + 1000 // Ensure longer message than timeout
        );

        await TrySendRequestAsync(url, body, httpAction);
    }

    private static async Task TrySendRequestAsync(string url, string body, HttpAction httpAction)
    {
        var errorMsg = GetRequestFailedMessage();

        try
        {
            await SendRequestAsync(url, body, httpAction);
        }
        catch (Newtonsoft.Json.JsonException ex)
        {
            _ = MessageService.ShowTextMessageAsync("Failed to parse headers", "Failure", StatusIcon.Failure);
            _ = AnimationService.InvalidHeaderAnimationAsync(AppState.EditorInput);
            Log.Error(ex, "Failed to parse headers:");
        }
        catch (HttpRequestException ex)
        {
            _ = MessageService.ShowTextMessageAsync(errorMsg, "Failure", StatusIcon.Failure);
            MessageService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
            Log.Error(ex, "HTTP request failed:");
        }
        catch (TaskCanceledException)
        {
            _ = MessageService.ShowTextMessageAsync(GetTimeoutMessage(), "Failure", StatusIcon.Failure);
            Log.Warning(GetTimeoutMessage());
        }
        catch (Exception ex)
        {
            _ = MessageService.ShowTextMessageAsync(errorMsg, "Failure", StatusIcon.Failure);
            MessageService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
            Log.Error(ex, "An unexpected error occurred:");
        }
    }

    private static async Task SendRequestAsync(string url, string body, HttpAction httpAction)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(Config.HttpConnectionTimeout)
        };

        var headers = GetHeaders();
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
        {
            Log.Debug("No headers provided");
            return await client.SendAsync(request);
        }

        // Headers found
        Log.Debug("Adding headers:");
        foreach (var header in headers)
        {
            if (request.Headers.TryAddWithoutValidation(header.Key, header.Value))
            {
                Log.Debug($"> {header.Key} = {header.Value}");
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

    private static Dictionary<string, string>? GetHeaders()
    {
        if (string.IsNullOrWhiteSpace(AppState.EditorInput.HeaderEditorText)) 
            return null;

        // Parse headers       
        var headers = JsonConvert.DeserializeObject<Dictionary<string, string>>(AppState.EditorInput.HeaderEditorText)
                    ?.Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return headers?.Count > 0 ? headers : null;
    }

    private static async Task SetHttpResponseText(HttpResponseMessage? response)
    {
        if (response == null || AppState.EditorInput == null)
            return;

        var contentType = response.Content?.Headers.ContentType?.MediaType;

        // JSON
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("Response is JSON");
            AppState.EditorInput.ResponseEditorText = await GetResponseAsJsonAsync(response); 
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Json;
            return;
        }

        // HTML
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("html", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("Response is HTML");
            AppState.EditorInput.ResponseEditorText = await response.Content!.ReadAsStringAsync();
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Html;
            return;
        }

        // XML
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("Response is XML");
            AppState.EditorInput.ResponseEditorText = await response.Content!.ReadAsStringAsync();
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Xml;
            return;
        }

        // Fallback
        Log.Debug("Response fallback: Plain Text");
        AppState.ResponseEditorSyntax = SyntaxHighlighting.PlainText;
        AppState.EditorInput.ResponseEditorText = await response.Content!.ReadAsStringAsync();
    }

    private static async Task<string> GetResponseAsJsonAsync(HttpResponseMessage response)
    {
        var responseText = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseText)) return "";
        
        using var jsonDoc = JsonDocument.Parse(responseText);
        
        return System.Text.Json.JsonSerializer.Serialize(
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
            _ = AnimationService.InvalidBorderAnimationAsync(AppState.UrlInput.UrlBorder);
            _ = MessageService.ShowTextMessageAsync("Please enter a valid URL!", "Failure", StatusIcon.Failure);
            return false;
        }

        if (httpAction == HttpAction.NONE)
        {
            _ = MessageService.ShowTextMessageAsync("Please choose an HTTP action!", "Failure", StatusIcon.Failure);
            _ = AnimationService.InvalidBorderAnimationAsync(AppState.MainWindow.HttpActionButtonsBorder);
            return false;
        }

        return true;
    }

    private static void ShowHttpResponseMessage(HttpResponseMessage response)
    {
        (var color, var icon) = response.IsSuccessStatusCode
            ? ("Success", StatusIcon.Success)
            : ("Failure", StatusIcon.Failure);

        var statusCode = (int)response.StatusCode;

        _ = MessageService.ShowTextMessageAsync(
            $"{statusCode}: {response.ReasonPhrase}",
            color,
            icon
        );
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
        return $"Request timed out after {Config.HttpConnectionTimeout} seconds!";
    }
    
    private static string GetSuccessMessage()
    {
        var httpAction = AppState.SelectedHttpAction.ToString();
        return $"{httpAction} request sent successfully!";
    }
}
