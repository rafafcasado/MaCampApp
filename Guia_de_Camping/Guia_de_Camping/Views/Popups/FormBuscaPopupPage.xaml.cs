using Aspbrasil.AppSettings;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormBuscaPopupPage : PopupPage
    {
        public FormBuscaPopupPage()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MENSAGEM_BUSCA_REALIZADA);
            MessagingCenter.Subscribe<Application>(this, AppConstants.MENSAGEM_BUSCA_REALIZADA, async (r) =>
            {
                await Navigation.PopPopupAsync();
            });

            TapGestureRecognizer fecharPopup = new TapGestureRecognizer();
            fecharPopup.Tapped += btFechar_Clicked;
            imFechar.GestureRecognizers.Add(fecharPopup);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MENSAGEM_BUSCA_REALIZADA);
        }

        private async void btFechar_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}