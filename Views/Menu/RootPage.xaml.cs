using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Views.Campings;
using MaCamp.Views.Detalhes;
using MaCamp.Views.Listagens;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;

namespace MaCamp.Views.Menu
{
    public partial class RootPage : FlyoutPage
    {
        private RootPageMaster Master { get; }

        public RootPage()
        {
            InitializeComponent();

            Master = new RootPageMaster
            {
                IconImageSource = new FileImageSource
                {
                    File = "slideout.png"
                }
            };

            Master.ListView.ItemSelected += ListView_ItemSelected;

            Flyout = Master;
            Detail = CriarPaginaDetalhes(new MainPage());
        }

        private async void ListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is not ItemMenu item || item.TipoLayout == TipoLayoutMenu.Divisoria)
            {
                return;
            }

            Master.ListView.SelectedItem = null;

            if (Detail is NavigationPage navigationPage && navigationPage.CurrentPage is MainPage mainPage)
            {
                switch (item.TipoAcao)
                {
                    case TipoAcaoMenu.Home:
                        break;
                    case TipoAcaoMenu.AbrirBuscaCamping:
                        mainPage.SelectedItem = null;
                        mainPage.SelectedItem = mainPage.Children[0];

                        //MessagingCenter.Send(Application.Current, AppConstants.MensagemExibirBuscaCampings);

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
                        await Detail.Navigation.PushAsync(new ListagemItensPage(AppConstants.UrlPegarPosts, item.TituloPagina, tag: "app-parceiros"));

                        break;
                    case TipoAcaoMenu.AbrirDicasCampismo:
                        await Detail.Navigation.PushAsync(new ListagemItensPage(AppConstants.UrlPegarPosts, item.TituloPagina, tag: "app-dicas"));

                        break;
                    case TipoAcaoMenu.AbrirMapa:
                        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        {
                            await DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");

                            return;
                        }

                        await Detail.Navigation.PushAsync(new MapaPage(false));

                        break;
                    case TipoAcaoMenu.CadastreUmCamping:
                        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        {
                            await DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");

                            return;
                        }

                        await Detail.Navigation.PushAsync(new CadastreUmCampingPage());

                        break;
                    case TipoAcaoMenu.AbrirSobreAEmpresa:
                        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        {
                            await DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");

                            return;
                        }

                        await Navigation.PushPopupAsync(new LoadingPopupPage(AppColors.CorPrimaria));
                        var itens = await new WebService<Item>().Get(AppConstants.UrlPegarPosts, 1, "app-sobre");
                        await Navigation.PopPopupAsync();

                        if (itens.Count == 0)
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
                        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        {
                            await DisplayAlert("Essa atualização requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");

                            return;
                        }

                        mainPage.SelectedItem = mainPage.Children[0];

                        //MessagingCenter.Send(Application.Current, AppConstants.MensagemBuscarCampingsAtualizados);

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
                        var instance = Activator.CreateInstance(item.TargetType);

                        if (instance is Page page)
                        {
                            page.Title = !string.IsNullOrWhiteSpace(item.TituloPagina) ? item.TituloPagina : item.Titulo;
                            Detail = CriarPaginaDetalhes(page);
                        }

                        break;
                }

                await Task.Delay(500).ContinueWith((task) => Dispatcher.Dispatch(() => IsPresented = false));
            }
        }

        private Page CriarPaginaDetalhes(Page page)
        {
            return new NavigationPage(page)
            {
                BarTextColor = Colors.White,
                BarBackgroundColor = AppColors.CorPrimaria
            };
        }
    }
}