using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Services;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.CustomViews;
using MaCamp.Views.Detalhes;
using MaCamp.Views.Popups;
using RGPopup.Maui.Extensions;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Campings
{
    public partial class ListagemCampingsView : SmartContentView
    {
        private ListagemInfinitaVM ViewModel { get; }
        private int PaginaAtual { get; set; }
        private List<int> IdsQueJaChamaramPaginacao { get; set; }
        private bool CarregandoItem { get; set; }
        private string EndpointListagem { get; }
        private string? Tag { get; }
        private string? ParametrosBusca { get; }
        //Controle da barra de busca e filtro
        private int IdUltimoItemExibido { get; set; }
        private bool ViewFiltroAberta { get; set; }

        public ListagemCampingsView()
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            ViewModel = new ListagemInfinitaVM();
            IdsQueJaChamaramPaginacao = new List<int>();
            EndpointListagem = string.Empty;
            ViewFiltroAberta = true;
            Tag = null;
            ParametrosBusca = null;
            cvItens.ItemTemplate = new ItemDataTemplateSelector();

            FirstAppeared += ListagemCampingsView_FirstAppeared;
        }

        public ListagemCampingsView(string endpointListagem, string? tag = null, string? parametrosBusca = null)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            ViewModel = new ListagemInfinitaVM();
            IdsQueJaChamaramPaginacao = new List<int>();
            EndpointListagem = endpointListagem;
            ViewFiltroAberta = true;
            Tag = tag;
            ParametrosBusca = parametrosBusca;
            cvItens.ItemTemplate = new ItemDataTemplateSelector();

            FirstAppeared += ListagemCampingsView_FirstAppeared;

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados, async (recipient, message) =>
            {
                var progressoVisual = new ProgressoVisual(progressBar);

                cvItens.IsVisible = false;
                grBotoesFiltroMapa.IsVisible = false;
                slBaixandoCampings.IsVisible = true;
                slMensagemAviso.IsVisible = false;
                DeviceDisplay.KeepScreenOn = true;

                await Task.WhenAll(
                    CidadesServices.AtualizarListaCidadesAsync(progressoVisual),
                    CampingServices.BaixarCampingsAsync(false, progressoVisual),
                    CarregarConteudoAsync(progressoVisual)
                );
            });

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de Campings");
        }

        private async void ListagemCampingsView_FirstAppeared(object? sender, EventArgs e)
        {
            var progressoVisual = new ProgressoVisual(progressBar);

            cvItens.IsVisible = false;
            grBotoesFiltroMapa.IsVisible = false;
            slBaixandoCampings.IsVisible = true;
            slMensagemAviso.IsVisible = false;
            DeviceDisplay.KeepScreenOn = true;

            await Task.WhenAll(
                CidadesServices.AtualizarListaCidadesAsync(progressoVisual),
                CampingServices.BaixarCampingsAsync(false, progressoVisual),
                CarregarConteudoAsync(progressoVisual)
            );
        }

        private async void Handle_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (sender is CollectionView collectionView && collectionView.ItemsSource is ObservableCollection<Item> itemsSource)
            {
                var item = itemsSource.ElementAtOrDefault(e.LastVisibleItemIndex);

                if (item != null)
                {
                    if (!ViewFiltroAberta && (item.IdLocal <= 2 || IdUltimoItemExibido > item.IdLocal))
                    {
                        await slFiltrosEBusca.TranslateTo(0, 0);

                        ViewFiltroAberta = true;
                    }
                    else if (ViewFiltroAberta && item.IdLocal > 2 && IdUltimoItemExibido < item.IdLocal)
                    {
                        await slFiltrosEBusca.TranslateTo(0, -slFiltrosEBusca.Height);

                        ViewFiltroAberta = false;
                    }

                    IdUltimoItemExibido = item.IdLocal;

                    //if (ViewModel.Itens != null && ViewModel.Itens.Count >= AppConstants.QUANTIDADE_NOTICIAS_POR_LOTE && ViewModel.Itens.Count - 5 > 0 && e.Item == ViewModel.Itens[ViewModel.Itens.Count - 5] && !IdsQueJaChamaramPaginacao.Contains(itemAtual.IdLocal))
                    //{
                    //    IdsQueJaChamaramPaginacao.Add(itemAtual.IdLocal);
                    //    CarregarConteudo(true);
                    //}
                }
            }
        }

        private async void Handle_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Item item && !CarregandoItem)
            {
                CarregandoItem = true;
                cvItens.SelectedItem = null;

                if (item.DeveAbrirExternamente && item.UrlExterna != null)
                {
                    await Launcher.OpenAsync(item.UrlExterna);
                }
                else
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        await ControladorDeAnuncios.VerificarEExibirAnuncioPopupAsync();
                    }

                    await Navigation.PushAsync(new DetalhesCampingPage(item));

                    CarregandoItem = false;
                }
            }
        }

        private async void RecarregarConteudo(object? sender, EventArgs e)
        {
            PaginaAtual = 0;
            slMensagemAviso.IsVisible = false;
            ViewModel.Itens = new ObservableCollection<Item>();
            cvItens.ItemsSource = new ObservableCollection<Item>();
            //IdsQueJaChamaramPaginacao = new List<int>();

            await CarregarConteudoAsync();
        }

        private async Task CarregarConteudoAsync(ProgressoVisual? progressoVisual = null)
        {
            ProgressoVisual.AumentarTotal(progressoVisual, 5);

            var existemCampingsBD = await CampingServices.ExistemCampingsAsync();

            ProgressoVisual.AumentarAtual(progressoVisual);

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                if (!existemCampingsBD)
                {
                    var fs = new FormattedString();

                    indicadorCarregamento.IsVisible = false;

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

                    return;
                }
            }

            ViewModel.Itens.Clear();

            slBaixandoCampings.IsVisible = !existemCampingsBD;
            DeviceDisplay.KeepScreenOn = !existemCampingsBD;
            indicadorCarregamento.IsVisible = PaginaAtual >= 0;

            if (!existemCampingsBD)
            {
                slMensagemAviso.IsVisible = false;
            }

            await Workaround.TaskWorkAsync(async () =>
            {
                await ViewModel.CarregarAsync(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca, TipoListagem.Camping);
            });

            ProgressoVisual.AumentarAtual(progressoVisual);

            var valorChaveUsarLocalizacaoUsuario = await DBContract.GetKeyValueAsync(AppConstants.Filtro_LocalizacaoSelecionada);
            var valorChaveBuscaCamping = await DBContract.GetKeyValueAsync(AppConstants.Filtro_NomeCamping);

            ProgressoVisual.AumentarAtual(progressoVisual);

            if (!string.IsNullOrWhiteSpace(valorChaveUsarLocalizacaoUsuario) && Convert.ToBoolean(valorChaveUsarLocalizacaoUsuario))
            {
                lbBuscaAtual.Text = "EXIBINDO OS VINTE CAMPINGS MAIS PRÓXIMOS A VOCÊ (20)";
            }
            else
            {
                var EstadoBD = await DBContract.GetKeyValueAsync(AppConstants.Filtro_EstadoSelecionado);
                var CIDADE_BD = await DBContract.GetKeyValueAsync(AppConstants.Filtro_CidadeSelecionada);
                var quantidadeAnuncios = ViewModel.Itens.Count(x => !x.EhAnuncio && !x.EhAdMobRetangulo);

                ProgressoVisual.AumentarAtual(progressoVisual);

                if (string.IsNullOrWhiteSpace(EstadoBD))
                {
                    lbBuscaAtual.Text = $"EXIBINDO CAMPINGS DE TODOS OS ESTADOS ({quantidadeAnuncios})";
                }
                else
                {
                    var cidade = string.IsNullOrWhiteSpace(CIDADE_BD) ? string.Empty : " - " + CIDADE_BD;

                    lbBuscaAtual.Text = $"EXIBINDO CAMPINGS EM: {EstadoBD.ToUpper()}{cidade.ToUpper()} ({quantidadeAnuncios})";
                }

                if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
                {
                    lbBuscaAtual.Text += $" COM NOME: {valorChaveBuscaCamping.ToUpper()}";
                }
            }

            slBaixandoCampings.IsVisible = false;
            rvItens.IsRefreshing = false;
            indicadorCarregamento.IsVisible = false;
            DeviceDisplay.KeepScreenOn = false;

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

            ProgressoVisual.AumentarAtual(progressoVisual);
        }

        private async void FiltrarListagem(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FiltrosPage());
        }

        private async void ExibirBusca(object sender, EventArgs e)
        {
            await Navigation.PushPopupAsync(new FormBuscaPopupPage());
        }

        private async void ExibirPaginaFiltros(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FiltrosPage(true));
        }

        private async void VerNoMapa(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await Navigation.PushAsync(new MapaPage(ViewModel.Itens));
            }
            else
            {
                await AppConstants.CurrentPage.DisplayAlert(AppConstants.Titulo_SemInternet, AppConstants.Descricao_SemInternet, "OK");
            }
        }
    }
}