using Newtonsoft.Json;

namespace to_lazy_to_curl.Settings;

static public class Config
{
    public const int StatusMessageDuration = 10000;          // Show status message for x ms
    public const long HttpConnectionTimeout = 6;            // Http timeout in Seconds
    public const double SplitEditorThreshold = 800;         // App width in px to switch editor layout

    public static readonly string PayloadSampleData =
        JsonConvert.SerializeObject(
            JsonConvert.DeserializeObject(@"
            {
                ""id"": 42,
                ""name"": ""Peppson"",
                ""email"": ""Peppson@hottestMail.com"",
                ""isActive"": true,
                ""projects"": [
                    { ""id"": 1, ""name"": ""Worst WPF App in History"", ""status"": ""Done-in-a-day-maybe"" },
                    { ""id"": 2, ""name"": ""ESP32 Shenanigans"", ""status"": ""Completed"" }
                ]
            }"),
            Formatting.Indented
    );

    public static readonly string PayloadStartupData =
        JsonConvert.SerializeObject(
            new
            {
                id = 42,
                foo = "bar",
                isActive = true,
            },
            Formatting.Indented
        );

    public static readonly string ResponseSampleData = "{}";
    public static readonly string ResponseStartupData = "{}";
    
    #if RELEASE
        public const string UrlSampleData = "https://httpbin.org/#/"; 
    #else
        public const string UrlSampleData = "https://localhost:7291/snus";
    #endif
}
