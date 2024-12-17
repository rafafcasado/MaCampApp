using RGPopup.Maui.Pages;

namespace MaCamp.Views.Popups
{
    public partial class LoadingPopupPage : PopupPage
    {
        public LoadingPopupPage(Color corLoader)
        {
            InitializeComponent();

            loader.Color = corLoader;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}