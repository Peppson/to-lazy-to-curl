using Newtonsoft.Json;

namespace to_lazy_to_curl.State;

static public class Config
{
    public const int MessageDuration = 5000; // Show status message for x ms
    public const long ConnectionTimeout = 5; // Http timout in Seconds
    public const double SplitEditorThreshold = 940; // App width in px to switch layout


    public const string UrlStartupData = "https://localhost:7291/snus"; // todo
    //public const string UrlStartupData = "https://jsonplaceholder.typicode.com/posts"; 

    
    public static readonly string JsonSampleData = JsonConvert.SerializeObject(
        JsonConvert.DeserializeObject(@"
        {
            ""id"": 42,
            ""name"": ""Peppson"",
            ""email"": ""Peppson@example.com"",
            ""isActive"": true,
            ""roles"": [""admin"", ""programmer""],
            ""projects"": [
                { ""id"": 1, ""name"": ""Worst WPF App in History"", ""status"": ""Done-in-a-day-maybe"" },
                { ""id"": 2, ""name"": ""ESP32 Shenanigans"", ""status"": ""Completed"" }
            ]
        }"),
        Formatting.Indented
    );
}
