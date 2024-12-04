using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Views.Campings;
using MaCamp.Views.Listagens;

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
            NavigationPage.SetBackButtonTitle(this, "");

            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    Dispatcher.Dispatch(async () =>
            //    {
            //        await DisplayAlert("Internet não disponível", "Verifique sua conexão e tente novamente.", "OK");
            //    })
            //    return;
            //}
            Children.Add(new CampingsPage()
            {
                IconImageSource = "icone_aba1.png"
            });

            Children.Add(new ListagemItensPage(AppConstants.UrlPegarPosts, "News", TipoListagem.Noticias, "app-noticias")
            {
                Title = "Campismo & Caravanismo",
                IconImageSource = "icone_aba2.png"
            });

            Children.Add(new ListagemItensPage()
            {
                Title = "Favoritos",
                IconImageSource = "icone_aba3.png"
            });

            Children.Add(new ListagemItensPage(AppConstants.UrlPegarPosts, "Eventos", TipoListagem.Noticias, "app-eventos")
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
            App.ExibirNotificacaoPush();
        }
    }
}