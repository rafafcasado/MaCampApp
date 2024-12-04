using Aspbrasil.AppSettings;
using Aspbrasil.DataAccess;
using Aspbrasil.Models;
using Aspbrasil.Views.Detalhes;
using Aspbrasil.Views.Popups;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views.Menu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage : MasterDetailPage
    {
        RootPageMaster _master = new RootPageMaster()
        {
            Icon = new FileImageSource { File = "slideout.png" },
        };

        public RootPage()
        {
            InitializeComponent();
            _master.ListView.ItemSelected += ListView_ItemSelected;
            Master = _master;
            Detail = CriarPaginaDetalhes(new MainPage());
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as ItemMenu;
            if (item == null || item.TipoLayout == TipoLayoutMenu.Divisoria) return;
            _master.ListView.SelectedItem = null;

            Page page = null;

            var mainPage = ((Detail as NavigationPage).CurrentPage as MainPage);
            switch (item.TipoAcao)
            {
                case TipoAcaoMenu.Home:
                    break;
                case TipoAcaoMenu.AbrirBuscaCamping:
                    mainPage.SelectedItem = null;
                    MessagingCenter.Send(App.Current, AppConstants.MENSAGEM_EXIBIR_BUSCA_CAMPINGS);
                    mainPage.SelectedItem = mainPage.Children[0];
                    break;
                case TipoAcaoMenu.AbrirNoticias:
                    mainPage.SelectedItem = null;
                    mainPage.SelectedItem = mainPage.Children[1];
                    break;
                case TipoAcaoMenu.Favoritos:
                    mainPage.SelectedItem = null;
                    mainPage.SelectedItem = mainPage.Children[2];
                    break;
                case TipoAcaoMenu.AbrirEventos:
                    mainPage.SelectedItem = null;
                    mainPage.SelectedItem = mainPage.Children[3];
                    break;
                case TipoAcaoMenu.AbrirParceiros:
                    await Detail.Navigation.PushAsync(new ListagemItensPage("https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts", item.TituloPagina, tag: "app-parceiros"));
                    break;
                case TipoAcaoMenu.AbrirDicasCampismo:
                    await Detail.Navigation.PushAsync(new ListagemItensPage("https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts", item.TituloPagina, tag: "app-dicas"));
                    break;
                case TipoAcaoMenu.AbrirMapa:
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        await DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");
                        return;
                    }
                    await Detail.Navigation.PushAsync(new MapaPage(false));
                    break;
                case TipoAcaoMenu.CadastreUmCamping:
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        await DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");
                        return;
                    }
                    await Detail.Navigation.PushAsync(new CadastreUmCampingPage());
                    break;
                case TipoAcaoMenu.AbrirSobreAEmpresa:
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        await DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");
                        return;
                    }

                    await Navigation.PushPopupAsync(new LoadingPopupPage(AppColors.COR_PRIMARIA));
                    List<Item> itens = await new WebService<Item>().Get("https://guiadecampings.homologacao.net/api/PostsAPI/GetPosts", 1, "app-sobre");
                    await Navigation.PopPopupAsync();
                    if (itens == null || itens.Count == 0)
                    {
                        await DisplayAlert("Falha", "Ocorreu um problema ao carregar a página. Tente novamente mais tarde.", "OK");
                    }
                    else
                    {
                        await Detail.Navigation.PushAsync(new DetalhesPage(itens[0]));
                    }
                    break;
                case TipoAcaoMenu.Item:
                    break;
                case TipoAcaoMenu.AtualizarCampings:
                    if (!CrossConnectivity.Current.IsConnected)
                    {
                        await DisplayAlert("Essa atualização requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");
                        return;
                    }
                    mainPage.SelectedItem = mainPage.Children[0];
                    MessagingCenter.Send(Application.Current, AppConstants.MENSAGEM_BUSCAR_CAMPINGS_ATUALIZADOS);
                    break;
                case TipoAcaoMenu.Configuracoes:
                    break;
                case TipoAcaoMenu.AbrirURI:
                    break;
                case TipoAcaoMenu.Sair:
                    break;
                case TipoAcaoMenu.Nenhuma:
                    break;
                case TipoAcaoMenu.NaoImplementadoNessaVersao:
                    break;
                default:
                    page = (Page)Activator.CreateInstance(item.TargetType);
                    if (!string.IsNullOrWhiteSpace(item.TituloPagina)) { page.Title = item.TituloPagina; }
                    else { page.Title = item.Titulo; }
                    Detail = CriarPaginaDetalhes(page);
                    break;
            }

            await Task.Delay(500).ContinueWith((task) => Device.BeginInvokeOnMainThread(() => IsPresented = false));
        }

        private Page CriarPaginaDetalhes(Page page)
        {
            return new NavigationPage(page)
            {
                BarTextColor = Color.White,
                BarBackgroundColor = AppColors.COR_PRIMARIA
            };
        }
    }
}