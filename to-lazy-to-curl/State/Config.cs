using Newtonsoft.Json;

namespace to_lazy_to_curl.State;

static public class Config
{
    public const int MessageDuration = 5000; // ms
    public const long ConnectionTimeout = 5; // Seconds
    public const string UrlStartupData = "https://localhost:7291/snus/test"; //"https://jsonplaceholder.typicode.com/posts"; //todo
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
