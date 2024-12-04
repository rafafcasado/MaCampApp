using Aspbrasil.Models.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aspbrasil.Models.Services
{
    public static class CidadesServices
    {
        public static async Task AtualizarListaCidades()
        {
            var DB = DBContract.NewInstance();

            using (var client = new HttpClient())
            {
                string url = "https://guiadecampings.homologacao.net/api/Cidades/GetCidades";
                string jsonCidades = string.Empty;
                try
                {
                    jsonCidades = await client.GetStringAsync(url);
                    List<Cidade> cidadesWS = JsonConvert.DeserializeObject<List<Cidade>>(jsonCidades).Where(x => !x.Estado.Contains("_")).ToList();

                    DB.ApagarCidades();
                    DB.InserirListaDeModelo(cidadesWS);
                }
                catch (Exception e)
                {
                    //TODO: Tratar exceção
                }
            }
        }
    }
}
