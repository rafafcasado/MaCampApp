using MaCamp.AppSettings;
using MaCamp.Models.Anuncios;
using Newtonsoft.Json;

namespace MaCamp.Models.DataAccess
{
    public class ConfiguracoesAnunciosDA
    {
        private static ConfiguracoesAnuncios? Configs { get; set; }

        public static async Task<ConfiguracoesAnuncios?> ObterConfigs(bool forcarAtualizacao = false)
        {
            if (Configs == null || forcarAtualizacao)
            {
                var jsonconfigs = await new HttpClient().GetStringAsync(AppConstants.UrlConfiguracoesAnuncios);
                Configs = JsonConvert.DeserializeObject<ConfiguracoesAnuncios>(jsonconfigs);
            }

            return Configs;
        }
    }
}