using MaCamp.AppSettings;
using MaCamp.Models.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Models.Services
{
    public static class CidadesServices
    {
        public static async Task AtualizarListaCidades()
        {
            try
            {
                var DB = DBContract.Instance;
                var listaCidades = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                if (listaCidades != null)
                {
                    DB.ApagarCidades();
                    DB.InserirListaDeModelo(listaCidades);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}