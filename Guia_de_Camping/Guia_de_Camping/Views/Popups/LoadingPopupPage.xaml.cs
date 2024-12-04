using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
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