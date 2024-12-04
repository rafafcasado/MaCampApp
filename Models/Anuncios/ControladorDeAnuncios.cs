using MaCamp.AppSettings;
using MaCamp.Models.DataAccess;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;

namespace MaCamp.Models.Anuncios
{
    public class ControladorDeAnuncios
    {
        public static async Task VerificarEExibirAnuncioPopup()
        {
            var quantidade = DBContract.NewInstance().ObterValorChave("QTD_ABERTURAS_DETALHES");

            if (int.TryParse(quantidade, out var quantidadeAberturasDetalhes))
            {
                var configuracoes = await ConfiguracoesAnunciosDA.ObterConfigs();

                if (configuracoes != null)
                {
                    var qtdNecessaria = configuracoes.QuantidadeAberturasPopup;

                    if (qtdNecessaria == quantidadeAberturasDetalhes)
                    {
                        await AppConstants.CurrentPage.Navigation.PushPopupAsync(new AnuncioPopupPage());
                        DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor("QTD_ABERTURAS_DETALHES", 0.ToString(), TipoChave.ControleInterno));
                    }
                    else
                    {
                        quantidadeAberturasDetalhes += 1;
                        DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor("QTD_ABERTURAS_DETALHES", quantidadeAberturasDetalhes.ToString(), TipoChave.ControleInterno));
                    }
                }
            }
        }
    }
}