using Newtonsoft.Json;

namespace to_lazy_to_curl.Settings;

static public class Config
{
    public const int StatusMessageDuration = 9000;      // Show status message for x ms
    public const long HttpConnectionTimeout = 5;        // Http timeout in Seconds
    public const double SplitEditorThreshold = 900;     // App width in px to switch editor layout
    public const string AppVersionNumber = "1.0.1";     // Hardcode that shit

    public static readonly string PayloadIsFirstBootData =
        JsonConvert.SerializeObject(
            new
            {
                id = 42,
                name = "peppson",
                isActive = true,
                projects = new[]
                {
                    new { id = 1, name = "random WPF app", status = "done-in-a-day-maybe" },
                    new { id = 2, name = "ESP32 shenanigans", status = "completed" }
                }
            },
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

    public static readonly string ResponseStartupData = "{}";

    public static readonly string HeaderStartupData =
        JsonConvert.SerializeObject(
            new
            {
                Authorization   = "",   // "Bearer <token>"
                Accept          = "",   // "application/json"
                AcceptLanguage  = "",   // "en-US,en;q=0.9"
                AcceptEncoding  = "",   // "gzip, deflate, br"
                UserAgent       = "",   // "MyApp/1.0"
                CacheControl    = "",   // "no-cache"
                Connection      = "",   // "keep-alive"
            },
            Formatting.Indented
        );

    #if RELEASE
        public const string UrlSampleData = "https://jsonplaceholder.typicode.com/posts"; // "https://httpbin.org/#/"
    #else
        public const string UrlSampleData = "https://localhost:7291/test";
    #endif
}
