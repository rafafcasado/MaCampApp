using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.Anuncios;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.Models.Services;
using Aspbrasil.ViewModels;
using Aspbrasil.Views.CustomCells;
using Aspbrasil.Views.Detalhes;
using Aspbrasil.Views.Popups;
using Plugin.Connectivity;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListagemCampingsView : ContentView
    {
        ListagemInfinitaVM _viewModel = new ListagemInfinitaVM();
        int PaginaAtual = 0;
        List<int> IdsQueJaChamaramPaginacao = new List<int>();

        string EndpointListagem;
        string Tag = "";
        string ParametrosBusca = "";

        //Controle da barra de busca e filtro
        int idUltimoItemExibido = 0;
        bool viewFiltroAberta = true, esconderPermitido = false, timerIniciado = false;


        public ListagemCampingsView(string endpointListagem, string tag = "", string parametrosBusca = "")
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Listagem de Campings");

            EndpointListagem = endpointListagem;
            Tag = tag;
            ParametrosBusca = parametrosBusca;


            //lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));
            lvItens.ItemTemplate = new ItemDataTemplateSelector();
            lvItens.ItemAppearing += Handle_ItemAppearing;
            //lvItens.RefreshCommand = new Command((obj) =>
            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MENSAGEM_BUSCAR_CAMPINGS_ATUALIZADOS);
            MessagingCenter.Subscribe<Application>(this, AppConstants.MENSAGEM_BUSCAR_CAMPINGS_ATUALIZADOS, (s) =>
            {
                Task.Run(async () =>
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        Device.BeginInvokeOnMainThread(() =>
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
                        Device.BeginInvokeOnMainThread(() =>
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

        bool carregandoItem = false;
        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Item;
            if (item != null && !carregandoItem)
            {
                carregandoItem = true;

                lvItens.SelectedItem = null;
                if (item.DeveAbrirExternamente)
                {
                    Device.OpenUri(new Uri(item.UrlExterna));
                }
                else
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        await ControladorDeAnuncios.VerificarEExibirAnuncioPopup();
                    }
                    await Navigation.PushAsync(new DetalhesCampingPage(item));
                    carregandoItem = false;
                }
            }
        }

        protected async void Handle_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (!esconderPermitido)
            {
                if (!timerIniciado)
                {
                    timerIniciado = true;
                    Device.StartTimer(TimeSpan.FromSeconds(3), () =>
                    {
                        esconderPermitido = true;
                        return false;
                    });
                }
                return;
            }

            var itemAtual = (e.Item as Item);

            if (!viewFiltroAberta && (itemAtual.IdLocal <= 2 || idUltimoItemExibido > itemAtual.IdLocal))
            {
                await slFiltrosEBusca.TranslateTo(0, 0);
                viewFiltroAberta = true;
            }
            else if (viewFiltroAberta && (itemAtual.IdLocal > 2 && idUltimoItemExibido < itemAtual.IdLocal))
            {
                await slFiltrosEBusca.TranslateTo(0, -slFiltrosEBusca.Height);
                viewFiltroAberta = false;
            }
            idUltimoItemExibido = itemAtual.IdLocal;

            //if (_viewModel.Itens != null
            //    && _viewModel.Itens.Count >= AppConstants.QUANTIDADE_NOTICIAS_POR_LOTE
            //    && _viewModel.Itens.Count - 5 > 0
            //    && e.Item == _viewModel.Itens[_viewModel.Itens.Count - 5]
            //    && !IdsQueJaChamaramPaginacao.Contains(itemAtual.IdLocal)
            //    )
            //{
            //    IdsQueJaChamaramPaginacao.Add(itemAtual.IdLocal);
            //    await Task.Run(() => CarregarConteudo());
            //}

        }

        private async void RecarregarConteudo(object sender, System.EventArgs e)
        {
            _viewModel.Itens = new System.Collections.ObjectModel.ObservableCollection<Item>();
            slMensagemAviso.IsVisible = false;
            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = true;
            }

            lvItens.ItemsSource = null;
            lvItens.ItemTemplate = new ItemDataTemplateSelector();
            //lvItens.ItemTemplate = new DataTemplate(typeof(CampingViewCell));

            PaginaAtual = 0;
            IdsQueJaChamaramPaginacao = new List<int>();
            await CarregarConteudo();
        }

        private async Task CarregarConteudo(bool forcandoAtualizacao = false)
        {
            bool existemCampingsBD = CampingServices.ExistemCampingsBD();


            if (!CrossConnectivity.Current.IsConnected)
            {
                if (forcandoAtualizacao)
                {
                    await App.Current.MainPage.DisplayAlert("Internet não disponível", "Para atualizar a listagem de campings, conecte-se à internet e tente novamente.", "OK");
                    return;
                }
                else if (!existemCampingsBD)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;
                        FormattedString fs = new FormattedString();
                        fs.Spans.Add(new Span { Text = "O primeiro acesso requer conexão com a internet.\n\n", FontAttributes = FontAttributes.Bold, FontSize = 20 });
                        fs.Spans.Add(new Span { Text = "Verifique sua conexão e toque para tentar novamente.\n(após o primeiro acesso, os campings estarão disponíveis off-line)" });
                        lbMensagemAviso.FormattedText = fs;
                        slMensagemAviso.IsVisible = true;
                    });
                    return;
                }
            }
            if (forcandoAtualizacao)
            {
                _viewModel.Itens.Clear();
            }

            if (PaginaAtual > 0)
            {
                Device.BeginInvokeOnMainThread(() => { loaderConteudoAdicional.IsVisible = true; });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    slBaixandoCampings.IsVisible = forcandoAtualizacao || !existemCampingsBD;
                    if (slBaixandoCampings.IsVisible)
                    {
                        slMensagemAviso.IsVisible = false;
                    }
                    loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoInicial.IsVisible = true;
                });
            }

            await _viewModel.Carregar(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca, TipoListagem.Camping);

            Device.BeginInvokeOnMainThread(() =>
            {
                var DB = DBContract.NewInstance();
                string valorChaveUsarLocalizacaoUsuario = DB.ObterValorChave("FILTROS_LOCALIZACAO_SELECIONADA");
                string valorChaveBuscaCamping = DB.ObterValorChave("FILTROS_NOME_DO_CAMPING");

                if (!string.IsNullOrWhiteSpace(valorChaveUsarLocalizacaoUsuario) && Convert.ToBoolean(valorChaveUsarLocalizacaoUsuario))
                {
                    lbBuscaAtual.Text = "EXIBINDO OS VINTE CAMPINGS MAIS PRÓXIMOS A VOCÊ (20)";
                }
                else
                {
                    string EstadoBD = DB.ObterValorChave("FILTROS_ESTADO_SELECIONADO");
                    string CIDADE_BD = DB.ObterValorChave("FILTROS_CIDADE_SELECIONADA");
                    if (string.IsNullOrWhiteSpace(EstadoBD))
                    {
                        lbBuscaAtual.Text = $"EXIBINDO CAMPINGS DE TODOS OS ESTADOS ({_viewModel.Itens.Count(x => !x.EhAnuncio && !x.EhAdMobRetangulo)})";
                    }
                    else
                    {
                        string cidade = (string.IsNullOrWhiteSpace(CIDADE_BD)) ? "" : " - " + CIDADE_BD;
                        lbBuscaAtual.Text = $"EXIBINDO CAMPINGS EM: {EstadoBD.ToUpper()}{cidade.ToUpper()} ({_viewModel.Itens.Count(x => !x.EhAnuncio && !x.EhAdMobRetangulo)})";
                    }
                    if (!string.IsNullOrWhiteSpace(valorChaveBuscaCamping))
                    {
                        lbBuscaAtual.Text += $" COM NOME: {valorChaveBuscaCamping.ToUpper()}";
                    }
                }
                slBaixandoCampings.IsVisible = false;

                lvItens.IsRefreshing = false;
                loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;
                if (_viewModel.Itens.Count > 0)
                {
                    slMensagemAviso.IsVisible = false;
                    lvItens.ItemsSource = _viewModel.Itens;
                    grFiltroMapa.IsVisible = true;
                    slAlterarFiltros.IsVisible = false;
                    lvItens.IsVisible = true;
                    grBotoesFiltroMapa.IsVisible = true;
                }
                else
                {
                    string mensagem = "Realize a busca novamente com outros critérios.";
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

                    FormattedString fs = new FormattedString();
                    fs.Spans.Add(new Span { Text = "Nenhum item disponível.\n\n", FontAttributes = FontAttributes.Bold, FontSize = 20 });
                    fs.Spans.Add(new Span { Text = mensagem });
                    lbMensagemAviso.FormattedText = fs;
                    slMensagemAviso.IsVisible = true;

                    slNovaBusca.IsVisible = true; 
                }
            });
        }

        private async void FiltrarListagem(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new FiltrosPage());
        }

        private async void ExibirBusca(object sender, System.EventArgs e)
        {
            await Navigation.PushPopupAsync(new FormBuscaPopupPage());
        }
        private async void ExibirPaginaFiltros(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new FiltrosPage(busca: true));
        }

        private async void VerNoMapa(object sender, System.EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await App.Current.MainPage.DisplayAlert("Este conteúdo requer conexão com a internet", "Verifique sua conexão e/ou tente novamente mais tarde.", "OK");
                return;
            }
            await Navigation.PushAsync(new MapaPage(_viewModel.Itens));
        }

    }
}