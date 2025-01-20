using MaCamp.Utils;
using MaCamp.Models.DataAccess;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;

namespace MaCamp.Models.Anuncios
{
    public static class ControladorDeAnuncios
    {
        public static async Task VerificarEExibirAnuncioPopup()
        {
            var quantidade = DBContract.Instance.ObterValorChave(AppConstants.Quantidade_AberturasDetalhes);

            if (int.TryParse(quantidade, out var quantidadeAberturasDetalhes))
            {
                var configuracoes = await ConfiguracoesAnunciosDA.ObterConfigs();

                if (configuracoes != null)
                {
                    var qtdNecessaria = configuracoes.QuantidadeAberturasPopup;

                    if (qtdNecessaria == quantidadeAberturasDetalhes)
                    {
                        await AppConstants.CurrentPage.Navigation.PushPopupAsync(new AnuncioPopupPage());

                        DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor(AppConstants.Quantidade_AberturasDetalhes, 0.ToString(), Enumeradores.TipoChave.ControleInterno));
                    }
                    else
                    {
                        quantidadeAberturasDetalhes += 1;

                        DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor(AppConstants.Quantidade_AberturasDetalhes, quantidadeAberturasDetalhes.ToString(), Enumeradores.TipoChave.ControleInterno));
                    }
                }
            }
        }
    }
}