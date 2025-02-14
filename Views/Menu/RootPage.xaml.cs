using MaCamp.Models;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
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

            Master.CollectionView.SelectionChanged += CollectionView_SelectionChanged;

            Flyout = Master;
            Detail = CriarPaginaDetalhes(new MainPage());
        }

        private async void CollectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is ItemMenu item && item.TipoLayout != Enumeradores.TipoLayoutMenu.Divisoria)
            {
                Master.CollectionView.SelectedItem = null;

                if (Detail is NavigationPage navigationPage && navigationPage.CurrentPage is MainPage mainPage)
                {
                    switch (item.TipoAcao)
                    {
                        case Enumeradores.TipoAcaoMenu.Home:
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirBuscaCamping:
                            mainPage.SelectedItem = null;
                            mainPage.SelectedItem = mainPage.Children[0];

                            MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_ExibirBuscaCampings);
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirNoticias:
                            mainPage.SelectedItem = null;
                            mainPage.SelectedItem = mainPage.Children[1];
                            break;
                        case Enumeradores.TipoAcaoMenu.Favoritos:
                            mainPage.SelectedItem = null;
                            mainPage.SelectedItem = mainPage.Children[2];
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirEventos:
                            mainPage.SelectedItem = null;
                            mainPage.SelectedItem = mainPage.Children[3];
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirParceiros:
                            await Detail.Navigation.PushAsync(new ListagemItensPage(AppConstants.Url_PegarPosts, item.TituloPagina, tag: "app-parceiros"));
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirDicasCampismo:
                            await Detail.Navigation.PushAsync(new ListagemItensPage(AppConstants.Url_PegarPosts, item.TituloPagina, tag: "app-dicas"));
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirMapa:
                            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                            //{
                            //    await DisplayAlert(AppConstants.Titulo_SemInternet, AppConstants.Descricao_SemInternet, "OK");

                            //    return;
                            //}

                            await Detail.Navigation.PushAsync(new MapaPage(false));
                            break;
                        case Enumeradores.TipoAcaoMenu.CadastreUmCamping:
                            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                            {
                                await DisplayAlert(AppConstants.Titulo_SemInternet, AppConstants.Descricao_SemInternet, "OK");

                                return;
                            }

                            await Detail.Navigation.PushAsync(new CadastreUmCampingPage());
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirSobreAEmpresa:
                            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                            {
                                await DisplayAlert(AppConstants.Titulo_SemInternet, AppConstants.Descricao_SemInternet, "OK");

                                return;
                            }

                            await Navigation.PushPopupAsync(new LoadingPopupPage(AppColors.CorPrimaria));

                            var itens = await new WebService().GetListAsync<Item>(AppConstants.Url_PegarPosts, 1, "app-sobre");

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
                        case Enumeradores.TipoAcaoMenu.Item:
                            break;
                        case Enumeradores.TipoAcaoMenu.AtualizarCampings:
                            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                            {
                                await DisplayAlert("Essa atualização requer conexão com a internet", AppConstants.Descricao_SemInternet, "OK");

                                return;
                            }

                            mainPage.SelectedItem = mainPage.Children[0];

                            MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_BuscarCampingsAtualizados);
                            break;
                        case Enumeradores.TipoAcaoMenu.Configuracoes:
                            break;
                        case Enumeradores.TipoAcaoMenu.AbrirURI:
                            break;
                        case Enumeradores.TipoAcaoMenu.Sair:
                            break;
                        case Enumeradores.TipoAcaoMenu.Nenhuma:
                            break;
                        case Enumeradores.TipoAcaoMenu.NaoImplementadoNessaVersao:
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

                    await Task.Delay(500).ContinueWith(task => Dispatcher.Dispatch(() => IsPresented = false));
                }
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