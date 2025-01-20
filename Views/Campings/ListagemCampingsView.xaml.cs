using System.Collections.ObjectModel;
using MaCamp.Utils;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Models.DataAccess;
using MaCamp.Models.Services;
using MaCamp.ViewModels;
using MaCamp.Views.CustomViews;
using MaCamp.Views.Detalhes;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;

namespace MaCamp.Views.Campings
{
    public partial class ListagemCampingsView : ContentView
    {
        private ListagemInfinitaVM ViewModel { get; }
        private int PaginaAtual { get; set; }
        //private List<int> IdsQueJaChamaramPaginacao { get; set; }
        private string EndpointListagem { get; }
        private string Tag { get; }
        private string ParametrosBusca { get; }
        private bool carregandoItem { get; set; }

        //Controle da barra de busca e filtro
        private int idUltimoItemExibido { get; set; }
        private bool viewFiltroAberta { get; set; }

        public ListagemCampingsView(string endpointListagem, string tag = "", string parametrosBusca = "")
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");
            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de Campings");

            ViewModel = new ListagemInfinitaVM();
            //IdsQueJaChamaramPaginacao = new List<int>();
            EndpointListagem = endpointListagem;
            viewFiltroAberta = true;
            Tag = tag;
            ParametrosBusca = parametrosBusca;

            cvItens.ItemTemplate = new ItemDataTemplateSelector();
            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MessagingCenter_BuscarCampingsAtualizados);

            MessagingCenter.Subscribe<Application>(this, AppConstants.MessagingCenter_BuscarCampingsAtualizados, s =>
            {
                Task.Run(async () =>
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            cvItens.IsVisible = false;
                            grBotoesFiltroMapa.IsVisible = false;
                            slBaixandoCampings.IsVisible = true;
                            slMensagemAviso.IsVisible = false;
                            loaderConteudoInicial.IsVisible = true;
                            loaderConteudoInicial.IsRunning = true;
                        });

                        await CidadesServices.AtualizarListaCidades();
                        await CampingServices.BaixarCampings(true);
                        await CarregarConteudo(true);
                    }
                    else
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            rvItens.IsRefreshing = false;
                            cvItens.IsVisible = true;
                            grBotoesFiltroMapa.IsVisible = true;
                            slBaixandoCampings.IsVisible = false;
                            slMensagemAviso.IsVisible = false;
                            loaderConteudoInicial.IsVisible = false;
                            loaderConteudoInicial.IsRunning = false;
                        });
                    }
                });
            });

            Task.Run(async () => await CarregarConteudo());
        }

        private async void Handle_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (sender is CollectionView collectionView && collectionView.ItemsSource is ObservableCollection<Item> itemsSource)
            {
                var item = itemsSource.ElementAtOrDefault(e.LastVisibleItemIndex);

                if (item != null)
                {
                    if (!viewFiltroAberta && (item.IdLocal <= 2 || idUltimoItemExibido > item.IdLocal))
                    {
                        await slFiltrosEBusca.TranslateTo(0, 0);

                        viewFiltroAberta = true;
                    }
                    else if (viewFiltroAberta && item.IdLocal > 2 && idUltimoItemExibido < item.IdLocal)
                    {
                        await slFiltrosEBusca.TranslateTo(0, -slFiltrosEBusca.Height);

                        viewFiltroAberta = false;
                    }

                    idUltimoItemExibido = item.IdLocal;

                    //if (ViewModel.Itens != null && ViewModel.Itens.Count >= AppConstants.QUANTIDADE_NOTICIAS_POR_LOTE && ViewModel.Itens.Count - 5 > 0 && e.Item == ViewModel.Itens[ViewModel.Itens.Count - 5] && !IdsQueJaChamaramPaginacao.Contains(itemAtual.IdLocal))
                    //{
                    //    IdsQueJaChamaramPaginacao.Add(itemAtual.IdLocal);
                    //    await Task.Run(() => CarregarConteudo());
                    //}
                }
            }
        }

        private async void Handle_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Item item && !carregandoItem)
            {
                carregandoItem = true;
                cvItens.SelectedItem = null;

                if (item.DeveAbrirExternamente && item.UrlExterna != null)
                {
                    await Launcher.OpenAsync(item.UrlExterna);
                }
                else
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        await ControladorDeAnuncios.VerificarEExibirAnuncioPopup();
                    }

                    await Navigation.PushAsync(new DetalhesCampingPage(item));

                    carregandoItem = false;
                }
            }
        }

        private async void RecarregarConteudo(object? sender, EventArgs e)
        {
            ViewModel.Itens = new ObservableCollection<Item>();
            slMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = true;
                loaderConteudoInicial.IsRunning = true;
            }

            cvItens.ItemsSource = new ObservableCollection<Item>();

            PaginaAtual = 0;
            //IdsQueJaChamaramPaginacao = new List<int>();
            await CarregarConteudo();
        }

        private async Task CarregarConteudo(bool forcandoAtualizacao = false)
        {
            var existemCampingsBD = CampingServices.ExistemCampingsBD();

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                if (forcandoAtualizacao)
                {
                    await AppConstants.CurrentPage.DisplayAlert(AppConstants.Titulo_SemInternet, "Para atualizar a listagem de campings, conecte-se à internet e tente novamente.", "OK");

                    return;
                }

                if (!existemCampingsBD)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        var fs = new FormattedString();

                        loaderConteudoInicial.IsVisible = false;
                        loaderConteudoInicial.IsRunning = false;
                        loaderConteudoAdicional.IsVisible = false;

                        fs.Spans.Add(new Span
                        {
                            Text = "O primeiro acesso requer conexão com a internet.\n\n",
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 20
                        });
                        fs.Spans.Add(new Span
                        {
                            Text = "Verifique sua conexão e toque para tentar novamente.\n(após o primeiro acesso, os campings estarão disponíveis off-line)"
                        });

                        lbMensagemAviso.FormattedText = fs;
                        slMensagemAviso.IsVisible = true;
                    });

                    return;
                }
            }

            if (forcandoAtualizacao)
            {
                ViewModel.Itens.Clear();
            }

            if (PaginaAtual > 0)
            {
                Dispatcher.Dispatch(() =>
                {
                    loaderConteudoAdicional.IsVisible = true;
                });
            }
            else
            {
                Dispatcher.Dispatch(() =>
                {
                    slBaixandoCampings.IsVisible = forcandoAtualizacao || !existemCampingsBD;

                    if (slBaixandoCampings.IsVisible)
                    {
                        slMensagemAviso.IsVisible = false;
                    }

                    loaderConteudoInicial.IsVisible = false;
                    loaderConteudoInicial.IsRunning = false;
                    loaderConteudoInicial.IsVisible = true;
                });
            }

            await ViewModel.Carregar(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca, Enumeradores.TipoListagem.Camping);

            Dispatcher.Dispatch(() =>
            {
                var DB = DBContract.Instance;
                var valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave(AppConstants.Filtro_LocalizacaoSelecionada);
                var valorChaveBuscaCamping = DB.ObterValorChave(AppConstants.Filtro_NomeCamping);

                if (!string.IsNullOrWhiteSpace(valorChaveUsarLocalizacaoUsuario) && Convert.ToBoolean(valorChaveUsarLocalizacaoUsuario))
                {
                    lbBuscaAtual.Text = "EXIBINDO OS VINTE CAMPINGS MAIS PRÓXIMOS A VOCÊ (20)";
                }
                else
                {
                    var EstadoBD = DB.ObterValorChave(AppConstants.Filtro_EstadoSelecionado);
                    var CIDADE_BD = DB.ObterValorChave(AppConstants.Filtro_CidadeSelecionada);
                    var quantidadeAnuncios = ViewModel.Itens.Count(x => !x.EhAnuncio && !x.EhAdMobRetangulo);

                    if (string.IsNullOrWhiteSpace(EstadoBD))
                    {
                        lbBuscaAtual.Text = $"EXIBINDO CAMPINGS DE TODOS OS ESTADOS ({quantidadeAnuncios})";
                    }
                    else
                    {
                        var cidade = string.IsNullOrWhiteSpace(CIDADE_BD) ? "" : " - " + CIDADE_BD;

                        lbBuscaAtual.Text = $"EXIBINDO CAMPINGS EM: {EstadoBD.ToUpper()}{cidade.ToUpper()} ({quantidadeAnuncios})";
                    }

                    if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
                    {
                        lbBuscaAtual.Text += $" COM NOME: {valorChaveBuscaCamping.ToUpper()}";
                    }
                }

                slBaixandoCampings.IsVisible = false;
                rvItens.IsRefreshing = false;
                loaderConteudoInicial.IsVisible = false;
                loaderConteudoInicial.IsRunning = false;
                loaderConteudoAdicional.IsVisible = false;

                if (ViewModel.Itens.Count > 0)
                {
                    slMensagemAviso.IsVisible = false;
                    cvItens.ItemsSource = ViewModel.Itens;
                    grFiltroMapa.IsVisible = true;
                    slAlterarFiltros.IsVisible = false;
                    cvItens.IsVisible = true;
                    grBotoesFiltroMapa.IsVisible = true;
                }
                else
                {
                    var mensagem = "Realize a busca novamente com outros critérios.";

                    cvItens.IsVisible = true;
                    grBotoesFiltroMapa.IsVisible = true;
                    grFiltroMapa.IsVisible = false;
                    grBuscaAtual.IsVisible = false;
                    slAlterarFiltros.IsVisible = false;

                    if (!App.EXISTEM_CAMPINGS_DISPONIVEIS)
                    {
                        mensagem = "Conecte-se à uma internet mais rápida e estável para baixar a lista de campings.";
                        cvItens.IsVisible = false;
                        grBotoesFiltroMapa.IsVisible = false;
                    }

                    var fs = new FormattedString();

                    fs.Spans.Add(new Span
                    {
                        Text = "Nenhum item disponível.\n\n",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });
                    fs.Spans.Add(new Span
                    {
                        Text = mensagem
                    });

                    lbMensagemAviso.FormattedText = fs;
                    slMensagemAviso.IsVisible = true;
                    slNovaBusca.IsVisible = true;
                }
            });
        }

        private async void FiltrarListagem(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FiltrosPage());
        }

        private async void ExibirBusca(object sender, EventArgs e)
        {
            await Navigation.PushPopupAsync(new FormBuscaPopupPage());
        }

        //private async void ExibirPaginaFiltros(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new FiltrosPage(true));
        //}

        private async void VerNoMapa(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await AppConstants.CurrentPage.DisplayAlert(AppConstants.Titulo_SemInternet, AppConstants.Descricao_SemInternet, "OK");

                return;
            }

            await Navigation.PushAsync(new MapaPage(ViewModel.Itens));
        }
    }
}