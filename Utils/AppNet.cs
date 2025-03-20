using System.Text;
using Newtonsoft.Json;

namespace MaCamp.Utils
{
    public static class AppNet
    {
        private static HttpClient Client { get; }

        static AppNet()
        {
            Client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(120)
            };
        }

        public static async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await Client.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<T>(response);

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
                var response = await Client.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<List<T>>(response);

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
                var json = JsonConvert.SerializeObject(data);
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