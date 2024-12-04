using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MensagemPopupPage : PopupPage
    {
        /// <summary>
        ///     Exibe um popup de mensagem, descendo do topo da tela.
        /// </summary>
        /// <param name="mensagem">Mensagem que será exibida no popup.</param>
        /// <param name="sucesso">True: Popup de sucesso. False: Mensagem de falha. null: Mensagem de aviso.</param>
        public MensagemPopupPage(string mensagem, bool? sucesso = null)
        {
            InitializeComponent();
            lbMensagem.Text = mensagem;

            if (sucesso.HasValue)
            {
                if (sucesso.Value) { frMensagem.BackgroundColor = Color.FromHex("#43A047"); frMensagem.BorderColor = Color.FromHex("#43A047"); }
                else { frMensagem.BackgroundColor = Color.FromHex("#B91919"); frMensagem.BorderColor = Color.FromHex("#B91919"); }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            HidePopup();
        }

        private async void HidePopup()
        {
            await Task.Delay(4000);
            await PopupNavigation.RemovePageAsync(this);
        }
    }
}