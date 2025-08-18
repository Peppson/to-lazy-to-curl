using System.Net.Http;
using System.Text;
using System.Text.Json;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Services;

public static class HttpService
{
    public static ICSharpCode.AvalonEdit.TextEditor? JsonResponseBody { get; set; }

    public static async Task SubmitRequestAsync(string url, string body)
    {
        if (!CanSubmitRequest(url, body)) return;

        _ = UiService.ShowMessageAsync("Sending...", "PrimaryText", 10000);

        var httpAction = AppState.SelectedHttpAction;
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
            _ = UiService.ShowMessageAsync(errorMsg, "Failure", Config.MessageDuration);
            UiService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
        }
        catch (TaskCanceledException)
        {
            _ = UiService.ShowMessageAsync(GetTimeoutMessage(), "Failure", Config.MessageDuration);
        }
        catch (Exception ex)
        {
            _ = UiService.ShowMessageAsync(errorMsg, "Failure", Config.MessageDuration);
            UiService.ShowMessageBox($"{errorMsg}:\n\n{ex.Message}");
        }
    }

    private static async Task SendRequestAsync(string url, string body, HttpAction httpAction)
    {
        using var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(Config.ConnectionTimeout)
        };

        // Set header and body
        var contentTypeHeader = AppState.PayloadEditorSyntax;
        var content = new StringContent(body, Encoding.UTF8, GetContentTypeHeader(contentTypeHeader));
        //var content = new StringContent(body, Encoding.UTF8, "application/json"); // todo add headers

        // Send it!
        var response = await SendHttpRequestAsync(client, httpAction, url, content);
        ShowHttpResponseMessage(response);
        await SetHttpResponseText(response);
    }

    private static async Task<HttpResponseMessage> SendHttpRequestAsync( // todo add headers
        HttpClient client,
        HttpAction action,
        string url,
        HttpContent?
        body = null) 
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
    
    private static string GetContentTypeHeader(string contentTypeHeader)
    {
        return contentTypeHeader switch
        {
            SyntaxHighlighting.Json => "application/json",
            SyntaxHighlighting.Html => "text/html",
            SyntaxHighlighting.Xml  => "application/xml",
            _ => "text/plain"
        };
    }

    private static async Task SetHttpResponseText(HttpResponseMessage? response)
    {
        if (response == null || JsonResponseBody == null)
            return;

        var contentType = response.Content?.Headers.ContentType?.MediaType;

        // JSON
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            JsonResponseBody.Text = await GetResponseAsJsonAsync(response);
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Json;
            return;
        }

        // HTML
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("html", StringComparison.OrdinalIgnoreCase))
        {
            JsonResponseBody.Text = await response.Content!.ReadAsStringAsync();
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Html;
            return;
        }

        // XML
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            JsonResponseBody.Text = await response.Content!.ReadAsStringAsync();
            AppState.ResponseEditorSyntax = SyntaxHighlighting.Xml;
            return;
        }

        // Fallback
        AppState.ResponseEditorSyntax = SyntaxHighlighting.PlainText;
        JsonResponseBody.Text = await response.Content!.ReadAsStringAsync();
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

    private static (bool IsUrlValid, bool IsPayloadValid) ValidateInputs(string url, string body)
    {
        bool isUrlValid = !string.IsNullOrWhiteSpace(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute);
        bool IsPayloadValid = true;
        //bool IsPayloadValid = !string.IsNullOrWhiteSpace(body);

        return (isUrlValid, IsPayloadValid);
    }

    private static bool GetIsBodyRequired(HttpAction action)
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

    private static bool CanSubmitRequest(string url, string body)
    {
        var (IsUrlValid, IsBodyValid) = ValidateInputs(url, body);

        if (!IsUrlValid)
        {
            _ = UiService.ShowMessageAsync("Please enter a valid URL!", "Failure", Config.MessageDuration);
            return false;
        }

        if (AppState.SelectedHttpAction == HttpAction.NONE)
        {
            _ = UiService.ShowMessageAsync("Please choose an HTTP action!", "Failure", Config.MessageDuration);
            return false;
        }

        var isBodyRequired = GetIsBodyRequired(AppState.SelectedHttpAction);

        if (isBodyRequired && !IsBodyValid)
        {
            _ = UiService.ShowMessageAsync(GetBodyRequiredMessage(), "Failure", Config.MessageDuration);
            return false;
        }

        // Let GET and DELETE pass through without content body
        if ((isBodyRequired && IsBodyValid) ||
            AppState.SelectedHttpAction == HttpAction.GET ||
            AppState.SelectedHttpAction == HttpAction.DELETE)
        {
            return true;
        }

        return false; // Should never get here
    }

    private static void ShowHttpResponseMessage(HttpResponseMessage response)
    {
        string color = response.IsSuccessStatusCode ? "Success" : "Failure";
        _ = UiService.ShowMessageAsync($"{(int)response.StatusCode}: {response.ReasonPhrase}", color, Config.MessageDuration);
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
        return $"{httpAction} request timed out after {Config.ConnectionTimeout} seconds!";
    }
    
    private static string GetSuccessMessage()
    {
        var httpAction = AppState.SelectedHttpAction.ToString();
        return $"{httpAction} request sent successfully!";
    }
}
