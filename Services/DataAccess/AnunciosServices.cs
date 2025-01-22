using MaCamp.Models.Anuncios;
using MaCamp.Utils;

namespace MaCamp.Services.DataAccess
{
    public static class AnunciosServices
    {
        public static async Task<List<Anuncio>> GetListAsync(bool force)
        {
            var key = typeof(Anuncio).Name;

            if (!force && AppConstants.DictionaryData.TryGetValue(key, out var value) && value is List<Anuncio> listaAnunciosSalvos)
            {
                return listaAnunciosSalvos;
            }

            var listaAnunciosNovos = await new WebService().GetListAsync<Anuncio>(AppConstants.Url_Anuncios, 1);

            AppConstants.DictionaryData[key] = listaAnunciosNovos;

            return listaAnunciosNovos;
        }
    }
}
