using System.Collections.ObjectModel;
using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Models.DataAccess;
using MaCamp.Models.Services;
using MaCamp.ViewModels;
using MaCamp.Views.CustomCells;
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
        private bool esconderPermitido { get; set; }
        private bool timerIniciado { get; set; }

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

            //lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));
            lvItens.ItemTemplate = new ItemDataTemplateSelector();
            lvItens.ItemAppearing += Handle_ItemAppearing;

            //lvItens.RefreshCommand = new Command((obj) =>
            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MensagemBuscarCampingsAtualizados);

            MessagingCenter.Subscribe<Application>(this, AppConstants.MensagemBuscarCampingsAtualizados, (s) =>
            {
                Task.Run(async () =>
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            lvItens.IsVisible = false;
                            grBotoesFiltroMapa.IsVisible = false;
                            slBaixandoCampings.IsVisible = true;
                            slMensagemAviso.IsVisible = false;
                            loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = true;
                        });

                        await CidadesServices.AtualizarListaCidades();
                        await CampingServices.BaixarCampings(true);
                        await CarregarConteudo(true);
                    }
                    else
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            lvItens.IsRefreshing = false;
                            lvItens.IsVisible = true;
                            grBotoesFiltroMapa.IsVisible = true;
                            slBaixandoCampings.IsVisible = false;
                            slMensagemAviso.IsVisible = false;
                            loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = false;
                        });
                    }
                });
            });

            Task.Run(async () => await CarregarConteudo());
        }

        private async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Item item && !carregandoItem)
            {
                carregandoItem = true;
                lvItens.SelectedItem = null;

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

        protected async void Handle_ItemAppearing(object? sender, ItemVisibilityEventArgs e)
        {
            if (!esconderPermitido)
            {
                if (!timerIniciado)
                {
                    timerIniciado = true;

                    Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
                    {
                        esconderPermitido = true;

                        return false;
                    });
                }

                return;
            }

            if (e.Item is Item itemAtual)
            {
                if (!viewFiltroAberta && (itemAtual.IdLocal <= 2 || idUltimoItemExibido > itemAtual.IdLocal))
                {
                    await slFiltrosEBusca.TranslateTo(0, 0);
                    viewFiltroAberta = true;
                }
                else if (viewFiltroAberta && itemAtual.IdLocal > 2 && idUltimoItemExibido < itemAtual.IdLocal)
                {
                    await slFiltrosEBusca.TranslateTo(0, -slFiltrosEBusca.Height);
                    viewFiltroAberta = false;
                }

                idUltimoItemExibido = itemAtual.IdLocal;

                //if (ViewModel.Itens != null
                //    && ViewModel.Itens.Count >= AppConstants.QUANTIDADE_NOTICIAS_POR_LOTE
                //    && ViewModel.Itens.Count - 5 > 0
                //    && e.Item == ViewModel.Itens[ViewModel.Itens.Count - 5]
                //    && !IdsQueJaChamaramPaginacao.Contains(itemAtual.IdLocal)
                //    )
                //{
                //    IdsQueJaChamaramPaginacao.Add(itemAtual.IdLocal);
                //    await Task.Run(() => CarregarConteudo());
                //}
            }
        }

        private async void RecarregarConteudo(object? sender, EventArgs e)
        {
            ViewModel.Itens = new ObservableCollection<Item>();
            slMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = true;
            }

            lvItens.ItemsSource = null;
            lvItens.ItemTemplate = new ItemDataTemplateSelector();

            //lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));
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
                    await AppConstants.CurrentPage.DisplayAlert("Internet não disponível", "Para atualizar a listagem de campings, conecte-se à internet e tente novamente.", "OK");

                    return;
                }

                if (!existemCampingsBD)
                {
                    Dispatcher.Dispatch(() =>
                    {
                        loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;
                        var fs = new FormattedString();

                        fs.Spans.Add(new Span
                        {
                            Text = "O primeiro acesso requer conexão com a internet.\n\n",
                            FontAttributes = FontAttributes.Bold, FontSize = 20
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

                    loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoInicial.IsVisible = true;
                });
            }

            await ViewModel.Carregar(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca, TipoListagem.Camping);

            Dispatcher.Dispatch(() =>
            {
                var DB = DBContract.NewInstance();
                var valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
                var valorChaveBuscaCamping = DB.ObterValorChave("FILTROS_NOME_DO_CAMPING");

                if (!string.IsNullOrWhiteSpace(valorChaveUsarLocalizacaoUsuario) && Convert.ToBoolean(valorChaveUsarLocalizacaoUsuario))
                {
                    lbBuscaAtual.Text = "EXIBINDO OS VINTE CAMPINGS MAIS PRÓXIMOS A VOCÊ (20)";
                }
                else
                {
                    var EstadoBD = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
                    var CIDADE_BD = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");

                    if (string.IsNullOrWhiteSpace(EstadoBD))
                    {
                        lbBuscaAtual.Text = $"EXIBINDO CAMPINGS DE TODOS OS ESTADOS ({ViewModel.Itens.Count(x => !x.EhAnuncio && !x.EhAdMobRetangulo)})";
                    }
                    else
                    {
                        var cidade = string.IsNullOrWhiteSpace(CIDADE_BD) ? "" : " - " + CIDADE_BD;
                        lbBuscaAtual.Text = $"EXIBINDO CAMPINGS EM: {EstadoBD.ToUpper()}{cidade.ToUpper()} ({ViewModel.Itens.Count(x => !x.EhAnuncio && !x.EhAdMobRetangulo)})";
                    }

                    if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
                    {
                        lbBuscaAtual.Text += $" COM NOME: {valorChaveBuscaCamping.ToUpper()}";
                    }
                }

                slBaixandoCampings.IsVisible = false;
                lvItens.IsRefreshing = false;
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;

                if (ViewModel.Itens.Count > 0)
                {
                    slMensagemAviso.IsVisible = false;
                    lvItens.ItemsSource = ViewModel.Itens;
                    grFiltroMapa.IsVisible = true;
                    slAlterarFiltros.IsVisible = false;
                    lvItens.IsVisible = true;
                    grBotoesFiltroMapa.IsVisible = true;
                }
                else
                {
                    var mensagem = "Realize a busca novamente com outros critérios.";
                    lvItens.IsVisible = true;
                    grBotoesFiltroMapa.IsVisible = true;
                    grFiltroMapa.IsVisible = false;
                    grBuscaAtual.IsVisible = false;
                    slAlterarFiltros.IsVisible = false;

                    if (!App.EXISTEM_CAMPINGS_DISPONIVEIS)
                    {
                        mensagem = "Conecte-se à uma internet mais rápida e estável para baixar a lista de campings.";
                        lvItens.IsVisible = false;
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
                await AppConstants.CurrentPage.DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");

                return;
            }

            await Navigation.PushAsync(new MapaPage(ViewModel.Itens));
        }
    }
}