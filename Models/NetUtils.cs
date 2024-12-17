using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace MaCamp.Models
{
    public class NetUtils
    {
        public static async Task<string> GetString(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(120);

                return await client.GetStringAsync(url);
            }
            catch (Exception e)
            {
                Debug.WriteLine("FALHOU: " + e);
            }

            return string.Empty;
        }

        public static async Task<string> PostModel(string url, object model)
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return string.Empty;
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(url);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null!;
            }
        }
    }
}