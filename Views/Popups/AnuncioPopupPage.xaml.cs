using MaCamp.Views.CustomViews;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Popups
{
    public partial class AnuncioPopupPage : PopupPage
    {
        public AnuncioPopupPage()
        {
            InitializeComponent();

            var fecharPopup = new TapGestureRecognizer();

            anuncioView.Content = new AnuncioView(TipoAnuncio.Popup);

            fecharPopup.Tapped += btFechar_Clicked;

            //imFechar.GestureRecognizers.Add(fecharPopup);
        }

        private async void btFechar_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}