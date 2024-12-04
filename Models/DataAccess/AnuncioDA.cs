using MaCamp.AppSettings;
using MaCamp.Models.Anuncios;

namespace MaCamp.Models.DataAccess
{
    public class AnuncioDA
    {
        private static List<Anuncio> _anuncios = new List<Anuncio>();

        public static async Task<List<Anuncio>> ObterAnuncios(bool forcarAtualizacao = false)
        {
            if (_anuncios.Count == 0 || forcarAtualizacao)
            {
                _anuncios = await new WebService<Anuncio>().Get(AppConstants.UrlAnuncios, 1);
            }

            return _anuncios;
        }
    }
}