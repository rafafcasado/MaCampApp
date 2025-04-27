using MaCamp.Utils;
using MaCamp.ViewModels;

namespace MaCamp.Views.Menu
{
    public partial class MasterPage : ContentPage
    {
        public CollectionView CollectionView { get; }

        public MasterPage()
        {
            InitializeComponent();

            BindingContext = new MasterPageViewModel();
            MenuItemsListView.ItemTemplate = new MenuTemplateSelector();
            CollectionView = MenuItemsListView;
        }

        private void AbrirPatrocinador(object sender, EventArgs e)
        {
            Launcher.OpenAsync(AppConstants.Url_EasyTransports);
        }
    }
}