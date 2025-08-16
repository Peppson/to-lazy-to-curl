using Newtonsoft.Json;

namespace to_lazy_to_curl.Services;

static public class StartupData
{
    public static string GetUrlSampleData()
    {
        return "https://localhost:7291/snus/test";
        // return "https://jsonplaceholder.typicode.com/posts"; todo
    }

    public static string GetJsonSampleData()
    {
        string jsonRaw = @"
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
        }";

        return JsonConvert.SerializeObject(
            JsonConvert.DeserializeObject(jsonRaw),
            Formatting.Indented
        );
    }
}
