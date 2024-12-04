using Aspbrasil.Models;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnuncioPopupPage : PopupPage
    {
        public AnuncioPopupPage()
        {
            InitializeComponent();

            anuncioView.Content = new AnuncioView(TipoAnuncio.Popup);

            TapGestureRecognizer fecharPopup = new TapGestureRecognizer();
            fecharPopup.Tapped += btFechar_Clicked;
            //imFechar.GestureRecognizers.Add(fecharPopup);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private async void btFechar_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}