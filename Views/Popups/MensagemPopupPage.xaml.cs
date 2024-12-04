using RGPopup.Maui.Pages;

namespace MaCamp.Views.Popups
{
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
                if (sucesso.Value)
                {
                    frMensagem.BackgroundColor = Color.FromArgb("#43A047");
                    frMensagem.Stroke = new SolidColorBrush(Color.FromArgb("#43A047"));
                }
                else
                {
                    frMensagem.BackgroundColor = Color.FromArgb("#B91919");
                    frMensagem.Stroke = new SolidColorBrush(Color.FromArgb("#B91919"));
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await HidePopup();
        }

        private async Task HidePopup()
        {
            await Task.Delay(4000);
            Navigation.RemovePage(this);
        }
    }
}