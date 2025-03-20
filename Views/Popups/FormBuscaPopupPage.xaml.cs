using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Utils;
using RGPopup.Maui.Extensions;
using RGPopup.Maui.Pages;

namespace MaCamp.Views.Popups
{
    public partial class FormBuscaPopupPage : PopupPage
    {
        public FormBuscaPopupPage()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada);
            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada, async (recipient, message) =>
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

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada);
        }

        private async void btFechar_Clicked(object? sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}