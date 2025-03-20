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

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);

                var listaCidades = await AppNet.GetListAsync<Cidade>(AppConstants.Url_ListaCidades, x => x.Estado != null && !x.Estado.Contains("_"));

                await ProgressoVisual.AumentarAtualAsync(progressoVisual);
                await DBContract.UpdateAsync(false, listaCidades, progressoVisual);
                await ProgressoVisual.AumentarAtualAsync(progressoVisual);
            }
            catch (Exception ex)
            {
                await AppConstants.CurrentPage.DisplayAlert("Erro ao atualizar lista de cidades", ex.Message, "OK");
            }
        }
    }
}