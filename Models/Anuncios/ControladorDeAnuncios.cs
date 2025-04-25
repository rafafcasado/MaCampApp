using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Models.Anuncios
{
    public static class ControladorDeAnuncios
    {
        public static async Task VerificarEExibirAnuncioPopupAsync()
        {
            var quantidade = DBContract.GetKeyValue(AppConstants.Quantidade_AberturasDetalhes);

            if (int.TryParse(quantidade, out var quantidadeAberturasDetalhes))
            {
                var configuracoes = await ConfiguracoesAnunciosServices.GetAsync(false);

                if (configuracoes != null)
                {
                    var qtdNecessaria = configuracoes.QuantidadeAberturasPopup;

                    if (qtdNecessaria == quantidadeAberturasDetalhes)
                    {
                        await AppConstants.CurrentPage.Navigation.PushPopupAsync(new AnuncioPopupPage());

                        DBContract.UpdateKeyValue(AppConstants.Quantidade_AberturasDetalhes, "0", TipoChave.ControleInterno);
                    }
                    else
                    {
                        quantidadeAberturasDetalhes += 1;

                        DBContract.UpdateKeyValue(AppConstants.Quantidade_AberturasDetalhes, quantidadeAberturasDetalhes.ToString(), TipoChave.ControleInterno);
                    }
                }
            }
        }
    }
}