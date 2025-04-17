using System.Text;
using System.Text.Json;

namespace MaCamp.Services
{
    public class GoogleAnalyticsService
    {
        private readonly HttpClient _httpClient;
        private const string MeasurementId = "G-XXXXXXX";
        private const string ApiSecret = "YOUR_API_SECRET";

        public GoogleAnalyticsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task TrackEventAsync(string eventName, Dictionary<string, object>? parameters = null)
        {
            var clientId = Preferences.Get("ga_client_id", null);
            if (string.IsNullOrWhiteSpace(clientId))
            {
                clientId = Guid.NewGuid().ToString();
                Preferences.Set("ga_client_id", clientId);
            }

            var body = new
            {
                client_id = clientId,
                events = new[]
                {
                    new
                    {
                        name = eventName,
                        parameters = parameters ?? new Dictionary<string, object>()
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://www.google-analytics.com/mp/collect?measurement_id={MeasurementId}&api_secret={ApiSecret}";

            await _httpClient.PostAsync(url, content);
        }
    }

}
