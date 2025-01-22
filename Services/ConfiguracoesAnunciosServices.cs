using MaCamp.Models.Anuncios;
using MaCamp.Utils;

namespace MaCamp.Services
{
    public static class ConfiguracoesAnunciosServices
    {
        public static async Task<ConfiguracoesAnuncios?> GetAsync(bool force)
        {
            var key = typeof(ConfiguracoesAnuncios).Name;

            if (!force && AppConstants.DictionaryData.TryGetValue(key, out var value) && value is ConfiguracoesAnuncios configuracoesAnunciosSalvo)
            {
                return configuracoesAnunciosSalvo;
            }

            var configuracoesAnunciosNovo = await AppNet.GetAsync<ConfiguracoesAnuncios>(AppConstants.Url_ConfiguracoesAnuncios);

            AppConstants.DictionaryData[key] = configuracoesAnunciosNovo;

            return configuracoesAnunciosNovo;
        }
    }
}
