using MaCamp.AppSettings;
using MaCamp.Models.DataAccess;
using Newtonsoft.Json;

namespace MaCamp.Models.Services
{
    public static class CidadesServices
    {
        public static async Task AtualizarListaCidades()
        {
            try
            {
                var DB = DBContract.NewInstance();
                using var client = new HttpClient();
                var jsonCidades = await client.GetStringAsync(AppConstants.UrlListaCidades);
                var cidadesWS = JsonConvert.DeserializeObject<List<Cidade>>(jsonCidades)?.Where(x => x.Estado != null && !x.Estado.Contains("_")).ToList();

                if (cidadesWS != null)
                {
                    DB.ApagarCidades();
                    DB.InserirListaDeModelo(cidadesWS);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}