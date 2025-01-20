using MaCamp.Models.Anuncios;
using MaCamp.Utils;

namespace MaCamp.Models.DataAccess
{
    public class ConfiguracoesAnunciosDA
    {
        private static ConfiguracoesAnuncios? Configs { get; set; }

        public static async Task<ConfiguracoesAnuncios?> ObterConfigs(bool forcarAtualizacao = false)
        {
            if (Configs == null || forcarAtualizacao)
            {
                var configs = await AppNet.GetAsync<ConfiguracoesAnuncios>(AppConstants.Url_ConfiguracoesAnuncios);

                Configs = configs;
            }

            return Configs;
        }
    }
}