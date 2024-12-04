using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage(int selected = 1)
        {
            InitializeComponent();
            Title = AppSettings.AppConstants.NOME_APP;

            NavigationPage.SetBackButtonTitle(this, "");
            //if (!CrossConnectivity.Current.IsConnected)
            //{
            //    Device.BeginInvokeOnMainThread(async () =>
            //    {
            //        await DisplayAlert("Internet não disponível", "Verifique sua conexão e tente novamente.", "OK");
            //    })
            //    return;
            //}

            Children.Add(new CampingsPage() { Icon = "icone_aba1.png" });
            Children.Add(new ListagemItensPage("https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts", "News", TipoListagem.Noticias, "app-noticias") { Title = "Campismo & Caravanismo", Icon = "icone_aba2.png" });
            Children.Add(new ListagemItensPage() { Title = "Favoritos", Icon = "icone_aba3.png" });
            Children.Add(new ListagemItensPage("https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts", "Eventos", TipoListagem.Noticias, "app-eventos") { Title = "Eventos", Icon = "icone_aba4.png" });

            SelectedItem = Children[selected];
            //ToolbarItems.Add(new ToolbarItem("Buscar", "icone_busca.png", () =>
            //{
            //    DependencyService.Get<CustomControls.AdMobInterstitial>().Exibir();
            //}));
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.ExibirNotificacaoPush();
        }

    }
}
