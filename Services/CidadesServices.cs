using MaCamp.Services.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Models.Services
{
    public static class CidadesServices
    {
        public static async Task AtualizarListaCidades(ProgressoVisual? progressoVisual = null)
        {
            try
            {
                ProgressoVisual.AumentarTotal(progressoVisual, 3);

                var listaCidades = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                if (listaCidades != null)
                {
                    DBContract.ApagarCidades();
                    await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                    DBContract.InserirListaDeModelo(listaCidades);
                    await ProgressoVisual.AumentarAtualAsync(progressoVisual);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}