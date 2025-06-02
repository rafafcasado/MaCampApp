using System.Text;
using System.Text.Json;

namespace MaCamp.Utils
{
    public static class AppNet
    {
        private static HttpClient Client { get; }
        private static JsonSerializerOptions JsonSerializerOptionsDefault { get; }

        static AppNet()
        {
            Client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(120)
            };
            JsonSerializerOptionsDefault = new JsonSerializerOptions
            {
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public static async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await Client.GetStreamAsync(url);
                var data = await JsonSerializer.DeserializeAsync<T>(response, JsonSerializerOptionsDefault);

                return data;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(AppNet), nameof(GetAsync), ex);
            }

            return default;
        }

        public static async Task<List<T>> GetListAsync<T>(string url, Func<T, bool>? predicate = null)
        {
            try
            {
                using var response = await Client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonSerializerOptionsDefault);

                if (data != null)
                {
                    if (predicate != null)
                    {
                        return data.Where(predicate).ToList();
                    }

                    return data;
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(AppNet), nameof(GetListAsync), ex);
            }

            return new List<T>();
        }

        public static async Task<byte[]> GetBytesAsync(string url)
        {
            try
            {
                return await Client.GetByteArrayAsync(url);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(AppNet), nameof(GetBytesAsync), ex);
            }

            return Array.Empty<byte>();
        }

        public static async Task<string> GetStringAsync(string url)
        {
            try
            {
                return await Client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(AppNet), nameof(GetStringAsync), ex);
            }

            return string.Empty;
        }

        public static async Task<bool> PostAsync<T>(string url, T data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await Client.PostAsync(url, content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(AppNet), nameof(PostAsync), ex);
            }

            return false;
        }
    }
}