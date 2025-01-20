using MaCamp.Utils;
using MaCamp.Views.Campings;
using MaCamp.Views.Listagens;
using AndroidSpecific = Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace MaCamp.Views
{
    public partial class MainPage : TabbedPage
    {
        public MainPage(int selected = 1)
        {
            InitializeComponent();

            Title = AppConstants.NomeApp;

            // BarItemColor = "#40000000"
            // BarSelectedItemColor = "White"

            SelectedTabColor = AppColors.CorTextoSobreCorPrimaria;
            UnselectedTabColor = AppColors.CorPrimariaEscura;

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            AndroidSpecific.TabbedPage.SetToolbarPlacement(this, AndroidSpecific.ToolbarPlacement.Bottom);
            AndroidSpecific.Application.SetWindowSoftInputModeAdjust(this, AndroidSpecific.WindowSoftInputModeAdjust.Resize);

            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    Dispatcher.Dispatch(async () =>
            //    {
            //        await DisplayAlert(AppConstants.Titulo_SemInternet, AppConstants.Descricao_SemInternet, "OK");
            //    })
            //    return;
            //}

            Children.Add(new CampingsPage
            {
                IconImageSource = "icone_aba1.png",
                Title = "Campings",
            });
            Children.Add(new ListagemItensPage(AppConstants.Url_PegarPosts, "News", Enumeradores.TipoListagem.Noticias, "app-noticias")
            {
                Title = "Campismo & Caravanismo",
                IconImageSource = "icone_aba2.png"
            });
            Children.Add(new ListagemItensPage
            {
                Title = "Favoritos",
                IconImageSource = "icone_aba3.png"
            });
            Children.Add(new ListagemItensPage(AppConstants.Url_PegarPosts, "Eventos", Enumeradores.TipoListagem.Noticias, "app-eventos")
            {
                Title = "Eventos",
                IconImageSource = "icone_aba4.png"
            });

            SelectedItem = Children[selected];

            //ToolbarItems.Add(new ToolbarItem("Buscar", "icone_busca.png", () =>
            //{
            //    DependencyService.Get<CustomControls.AdMobInterstitial>().Exibir();
            //}));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var value = DeviceInfo.Platform == DevicePlatform.Android ? 34 : 0;

            this.Padding = new Thickness(0, value, 0, value);

            App.ExibirNotificacaoPush();
        }
    }
}