using MaCamp.AppSettings;
using MaCamp.Models.DataAccess;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;

namespace MaCamp.Models.Anuncios
{
    public static class ControladorDeAnuncios
    {
        public static async Task VerificarEExibirAnuncioPopup()
        {
            var quantidade = DBContract.Instance.ObterValorChave("QTD_ABERTURAS_DETALHES");

            if (int.TryParse(quantidade, out var quantidadeAberturasDetalhes))
            {
                var configuracoes = await ConfiguracoesAnunciosDA.ObterConfigs();

                if (configuracoes != null)
                {
                    var qtdNecessaria = configuracoes.QuantidadeAberturasPopup;

                    if (qtdNecessaria == quantidadeAberturasDetalhes)
                    {
                        await AppConstants.CurrentPage.Navigation.PushPopupAsync(new AnuncioPopupPage());
                        DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor("QTD_ABERTURAS_DETALHES", 0.ToString(), TipoChave.ControleInterno));
                    }
                    else
                    {
                        quantidadeAberturasDetalhes += 1;
                        DBContract.Instance.InserirOuSubstituirModelo(new ChaveValor("QTD_ABERTURAS_DETALHES", quantidadeAberturasDetalhes.ToString(), TipoChave.ControleInterno));
                    }
                }
            }
        }
    }
}