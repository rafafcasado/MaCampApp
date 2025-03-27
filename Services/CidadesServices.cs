using MaCamp.Models;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Services
{
    public static class CidadesServices
    {
        public static async Task AtualizarListaCidades(ProgressoVisual? progressoVisual = null)
        {
            try
            {
                ProgressoVisual.AumentarTotal(progressoVisual, 3);

                ProgressoVisual.AumentarAtual(progressoVisual);

                var listaCidades = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                ProgressoVisual.AumentarAtual(progressoVisual);

                DBContract.Update(false, listaCidades, progressoVisual);

                ProgressoVisual.AumentarAtual(progressoVisual);
            }
            catch (Exception ex)
            {
                await AppConstants.CurrentPage.DisplayAlert("Erro ao atualizar lista de cidades", ex.Message, "OK");
            }
        }
    }
}