using MaCamp.AppSettings;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;

namespace MaCamp.Views.Popups
{
    public partial class FormBuscaPopupPage : PopupPage
    {
        public FormBuscaPopupPage()
        {
            InitializeComponent();

            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MensagemBuscaRealizada);
            MessagingCenter.Subscribe<Application>(this, AppConstants.MensagemBuscaRealizada, async (r) =>
            {
                await Navigation.PopPopupAsync();
            });

            var fecharPopup = new TapGestureRecognizer();

            fecharPopup.Tapped += btFechar_Clicked;
            imFechar.GestureRecognizers.Add(fecharPopup);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MensagemBuscaRealizada);
        }

        private async void btFechar_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}