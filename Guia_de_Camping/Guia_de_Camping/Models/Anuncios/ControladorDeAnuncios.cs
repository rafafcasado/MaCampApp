using Aspbrasil.Models.DataAccess;
using Aspbrasil.Views.Popups;
using Rg.Plugins.Popup.Extensions;
using System.Threading.Tasks;

namespace Aspbrasil.Models.Anuncios
{
    public class ControladorDeAnuncios
    {
        public async static Task VerificarEExibirAnuncioPopup()
        {
            string qtd = DBContract.NewInstance().ObterValorChave("QTD_ABERTURAS_DETALHES");
            int qtdAberturasDetalhes = 1;
            int.TryParse(qtd, out qtdAberturasDetalhes);

            int qtdNecessaria = (await ConfiguracoesAnunciosDA.ObterConfigs()).QuantidadeAberturasPopup;
            if (qtdNecessaria == qtdAberturasDetalhes)
            {
                await App.Current.MainPage.Navigation.PushPopupAsync(new AnuncioPopupPage());
                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor("QTD_ABERTURAS_DETALHES", (0).ToString(), TipoChave.ControleInterno));
            }
            else
            {
                qtdAberturasDetalhes += 1;
                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor("QTD_ABERTURAS_DETALHES", qtdAberturasDetalhes.ToString(), TipoChave.ControleInterno));
            }
        }
    }
}
