using Newtonsoft.Json;

namespace to_lazy_to_curl.Settings;

static public class Config
{
    public const int MessageDuration = 8000;                    // Show status message for x ms
    public const long ConnectionTimeout = 6;                    // Http timeout in Seconds
    public const double SplitEditorThreshold = 800;             // App width in px to switch editor layout

    public static readonly string PayloadSampleData =
        JsonConvert.SerializeObject(
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

    public static readonly string ResponseSampleData = "{}";
    
    #if RELEASE
        public const string UrlStartupData = "https://httpbin.org/#/"; 
    #else
        public const string UrlStartupData = "https://localhost:7291/snus";
    #endif
}
