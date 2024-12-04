using System.Collections.ObjectModel;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Models.Services;
using MaCamp.ViewModels;
using MaCamp.Views.CustomCells;
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
            lvItens.Header = null;
            lvItens.ItemAppearing += Handle_ItemAppearing;
            lvItens.ItemTemplate = new ItemDataTemplateSelector();

            //lvItens.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            lvItens.RefreshCommand = new Command(() => RecarregarConteudo(null, null));

            Task.Run(async () => await CarregarConteudo());
        }

        private async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Item item)
            {
                lvItens.SelectedItem = null;

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

        protected async void Handle_ItemAppearing(object? sender, ItemVisibilityEventArgs e)
        {
            if (e.Item is Item item)
            {
                var temMaisItens = _viewModel.Itens.Count >= AppConstants.QuantidadeNoticiasPorLote;
                var contemItemLocal = !IdsQueJaChamaramPaginacao.Contains(item.IdLocal);

                if (temMaisItens && _viewModel.Itens.Count - 5 > 0 && e.Item == _viewModel.Itens[^5] && contemItemLocal)
                {
                    IdsQueJaChamaramPaginacao.Add(item.IdLocal);

                    await Task.Run(CarregarConteudo);
                }
            }
        }

        private async void RecarregarConteudo(object? sender, EventArgs? e)
        {
            _viewModel.Itens = new ObservableCollection<Item>();
            lbMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = true;
            }

            lvItens.ItemsSource = null;
            lvItens.ItemTemplate = new ItemDataTemplateSelector();

            //lvItens.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
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
                    loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;

                    //Verifica se existem Campings baixados
                    var existemCampingsBD = CampingServices.ExistemCampingsBD();
                    var fs = new FormattedString();

                    fs.Spans.Add(new Span
                    {
                        Text = "Este conteúdo requer conexão com a internet.\n\n", FontAttributes = FontAttributes.Bold,
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

                        var tap = new TapGestureRecognizer();

                        tap.Tapped += delegate
                        {
                            //MessagingCenter.Send(Application.Current, AppConstants.MensagemExibirBuscaCampings);

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
                            Text = "Verifique sua conexão e toque para tentar novamente."
                        });

                        var tap = new TapGestureRecognizer();
                        tap.Tapped += RecarregarConteudo;
                        lbMensagemAviso.GestureRecognizers.Add(tap);
                    }

                    lbMensagemAviso.FormattedText = fs;
                    lbMensagemAviso.IsVisible = true;
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
                    loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoInicial.IsVisible = true;
                });
            }

            await _viewModel.Carregar(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca, TipoListagem.Noticias);

            Dispatcher.Dispatch(() =>
            {
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;
                lvItens.IsRefreshing = false;

                if (_viewModel.Itens.Count > 0)
                {
                    lbMensagemAviso.IsVisible = false;
                    lvItens.ItemsSource = _viewModel.Itens;
                    lvItens.IsVisible = true;
                }
                else
                {
                    var fs = new FormattedString();

                    fs.Spans.Add(new Span
                    {
                        Text = "Nenhum item disponível.\n\n",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });

                    fs.Spans.Add(new Span
                    {
                        Text = "Confira sua internet e/ou tente novamente mais tarde."
                    });

                    lbMensagemAviso.FormattedText = fs;
                    lbMensagemAviso.IsVisible = true;
                    var tap = new TapGestureRecognizer();
                    tap.Tapped += RecarregarConteudo;
                    lbMensagemAviso.GestureRecognizers.Add(tap);
                }
            });
        }
    }
}