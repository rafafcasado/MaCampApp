using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Aspbrasil.Models
{
    public class NetUtils
    {
        public static async Task<string> GetString(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(120);
                    return await client.GetStringAsync(url);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("FALHOU: " + e.ToString());
            }
            return string.Empty;
        }

        public static async Task<string> PostModel(string url, object model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    return response;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}