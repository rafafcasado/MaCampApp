using System.Collections.ObjectModel;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Models.Services;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.CustomViews;
using MaCamp.Views.Detalhes;
using MaCamp.Views.Menu;

namespace MaCamp.Views.Listagens
{
    public partial class ListagemItensOnlineView : ContentView
    {
        private ListagemInfinitaVM _viewModel { get; }
        private int PaginaAtual;
        private List<int> IdsQueJaChamaramPaginacao { get; set; }
        private string EndpointListagem { get; }
        private string Tag { get; }
        private string ParametrosBusca { get; }

        public ListagemItensOnlineView(string endpointListagem, string tag = "", string parametrosBusca = "")
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, "");

            _viewModel = new ListagemInfinitaVM();
            IdsQueJaChamaramPaginacao = new List<int>();
            EndpointListagem = endpointListagem;
            Tag = tag;
            ParametrosBusca = parametrosBusca;

            cvItens.ItemTemplate = new ItemDataTemplateSelector();

            Task.Run(async () => await CarregarConteudo());
        }

        private async void Handle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Item item)
            {
                cvItens.SelectedItem = null;

                if (item.DeveAbrirExternamente && item.UrlExterna != null)
                {
                    await Launcher.OpenAsync(item.UrlExterna);
                }
                else
                {
                    await ControladorDeAnuncios.VerificarEExibirAnuncioPopup();
                    await Navigation.PushAsync(new DetalhesPage(item));
                }
            }
        }

        private async void Handle_Scrolled(object? sender, ItemsViewScrolledEventArgs e)
        {
            if (sender is CollectionView collectionView && collectionView.ItemsSource is ObservableCollection<Item> itemsSource)
            {
                var item = itemsSource.ElementAtOrDefault(e.LastVisibleItemIndex);

                if (item != null)
                {
                    var temMaisItens = _viewModel.Itens.Count >= AppConstants.QuantidadeNoticiasPorLote;
                    var contemItemLocal = !IdsQueJaChamaramPaginacao.Contains(item.IdLocal);

                    if (temMaisItens && _viewModel.Itens.Count - 5 > 0 && item == _viewModel.Itens[^5] && contemItemLocal)
                    {
                        IdsQueJaChamaramPaginacao.Add(item.IdLocal);

                        await Task.Run(CarregarConteudo);
                    }
                }
            }
        }

        private async void Handle_Refreshing(object? sender, EventArgs? e)
        {
            _viewModel.Itens = new ObservableCollection<Item>();
            lbMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = true;
                loaderConteudoInicial.IsRunning = true;
            }

            cvItens.ItemsSource = null;

            //lvItens.ItemTemplate = new DataTemplate(typeof(ItemContentView));
            PaginaAtual = 0;
            IdsQueJaChamaramPaginacao = new List<int>();

            await CarregarConteudo();
        }

        private async Task CarregarConteudo()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Dispatcher.Dispatch(() =>
                {
                    loaderConteudoInicial.IsVisible = false;
                    loaderConteudoInicial.IsRunning = false;
                    loaderConteudoAdicional.IsVisible = false;

                    //Verifica se existem Campings baixados
                    var existemCampingsBD = CampingServices.ExistemCampingsBD();
                    var fs = new FormattedString();
                    var tap = new TapGestureRecognizer();

                    fs.Spans.Add(new Span
                    {
                        Text = $"{AppConstants.Titulo_SemInternet}.\n\n",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });

                    if (existemCampingsBD)
                    {
                        fs.Spans.Add(new Span
                        {
                            Text = "Se preferir acesse o guia de campings, disponível off-line!"
                        });
                        fs.Spans.Add(new Span
                        {
                            Text = "\nToque aqui para acessar",
                            TextColor = AppColors.CorDestaque,
                            FontAttributes = FontAttributes.Bold
                        });

                        tap.Tapped += delegate
                        {
                            MessagingCenter.Send(Application.Current, AppConstants.MessagingCenter_ExibirBuscaCampings);

                            if (AppConstants.CurrentPage is RootPage rootPage && rootPage.Detail is NavigationPage navigationPage && navigationPage.CurrentPage is MainPage mainPage)
                            {
                                mainPage.SelectedItem = null;
                                mainPage.SelectedItem = mainPage.Children[0];
                            }
                        };

                        lbMensagemAviso.GestureRecognizers.Add(tap);
                    }
                    else
                    {
                        fs.Spans.Add(new Span
                        {
                            Text = AppConstants.Descricao_SemInternet
                        });

                        tap.Tapped += Handle_Refreshing;
                    }

                    lbMensagemAviso.FormattedText = fs;
                    lbMensagemAviso.IsVisible = true;

                    lbMensagemAviso.GestureRecognizers.Add(tap);
                });

                return;
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
                    loaderConteudoInicial.IsVisible = true;
                    loaderConteudoInicial.IsRunning = true;
                    loaderConteudoInicial.IsVisible = true;
                });
            }

            await _viewModel.Carregar(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca);

            Dispatcher.Dispatch(() =>
            {
                loaderConteudoInicial.IsVisible = false;
                loaderConteudoInicial.IsRunning = false;
                loaderConteudoAdicional.IsVisible = false;
                rvItens.IsRefreshing = false;

                if (_viewModel.Itens.Count > 0)
                {
                    lbMensagemAviso.IsVisible = false;
                    cvItens.ItemsSource = _viewModel.Itens;
                    cvItens.IsVisible = true;
                }
                else
                {
                    var formattedString = new FormattedString();

                    formattedString.Spans.Add(new Span
                    {
                        Text = "Nenhum item disponível.\n\n",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });

                    formattedString.Spans.Add(new Span
                    {
                        Text = "Confira sua internet e/ou tente novamente mais tarde."
                    });

                    lbMensagemAviso.FormattedText = formattedString;
                    lbMensagemAviso.IsVisible = true;

                    var tap = new TapGestureRecognizer();

                    tap.Tapped += Handle_Refreshing;

                    lbMensagemAviso.GestureRecognizers.Add(tap);
                }
            });
        }
    }
}