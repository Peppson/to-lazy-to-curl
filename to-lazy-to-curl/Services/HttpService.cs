using System.Net.Http;
using System.Text;
using to_lazy_to_curl.Models;
using to_lazy_to_curl.State;

namespace to_lazy_to_curl.Services;

public static class HttpService
{
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
            _ = UiService.ShowMessageAsync(GetSuccessMessage(), "Success", _messageDuration);
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

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content); // this 



        /* if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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

        return (true, null); */
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
