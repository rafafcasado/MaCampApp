using MaCamp.Utils;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;

namespace MaCamp.Views.Popups
{
    public partial class AnuncioPopupPage : PopupPage
    {
        public AnuncioPopupPage()
        {
            InitializeComponent();

            var fecharPopup = new TapGestureRecognizer();

            anuncioView.Content = new AnuncioView(Enumeradores.TipoAnuncio.Popup);

            fecharPopup.Tapped += btFechar_Clicked;

            //imFechar.GestureRecognizers.Add(fecharPopup);
        }

        private async void btFechar_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}