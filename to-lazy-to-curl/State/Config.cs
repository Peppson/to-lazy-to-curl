using Newtonsoft.Json;

namespace to_lazy_to_curl.State;

static public class Config
{
    public const int MessageDuration = 8000; // Show status message for x ms
    public const long ConnectionTimeout = 6; // Http timout in Seconds
    public const double SplitEditorThreshold = 800; // App width in px to switch layout

    public static readonly string JsonSampleData = JsonConvert.SerializeObject(
        JsonConvert.DeserializeObject(@"
        {
            ""id"": 42,
            ""name"": ""Peppson"",
            ""email"": ""Peppson@hottestmail.com"",
            ""isActive"": true,
            ""roles"": [""admin"", ""programmer""],
            ""projects"": [
                { ""id"": 1, ""name"": ""Worst WPF App in History"", ""status"": ""Done-in-a-day-maybe"" },
                { ""id"": 2, ""name"": ""ESP32 Shenanigans"", ""status"": ""Completed"" }
            ]
        }"),
        Formatting.Indented
    );

    public static readonly string JsonSampleResponse = JsonConvert.SerializeObject(
        JsonConvert.DeserializeObject(@"
        {
            ""status"": ""success"",
            ""message"": ""Data processed successfully!"",
            ""timestamp"": ""2025-08-17T12:34:56Z"",
            ""records"": [
                { ""id"": 101, ""name"": ""Tungis"", ""status"": ""active"" },
                { ""id"": 102, ""name"": ""Flutter"", ""status"": ""inactive"" }
            ]
        }"),
        Formatting.Indented
    );
    
    #if RELEASE
        public const string UrlStartupData = "https://httpbin.org/#/"; 
    #else
        public const string UrlStartupData = "https://localhost:7291/snus";
    #endif
}
