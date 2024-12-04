using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.Anuncios;
using Aspbrasil.Models.Services;
using Aspbrasil.ViewModels;
using Aspbrasil.Views.CustomCells;
using Aspbrasil.Views.Detalhes;
using Aspbrasil.Views.Menu;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListagemItensOnlineView : ContentView
    {
        ListagemInfinitaVM _viewModel = new ListagemInfinitaVM();
        int PaginaAtual = 0;
        List<int> IdsQueJaChamaramPaginacao = new List<int>();

        string EndpointListagem;
        string Tag = "";
        string ParametrosBusca = "";

        public ListagemItensOnlineView(string endpointListagem, string tag = "", string parametrosBusca = "")
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            EndpointListagem = endpointListagem;
            Tag = tag;
            ParametrosBusca = parametrosBusca;

            lvItens.Header = null;
            lvItens.ItemAppearing += Handle_ItemAppearing;

            lvItens.ItemTemplate = new ItemDataTemplateSelector();
            //lvItens.ItemTemplate = new DataTemplate(typeof(ItemViewCell));
            lvItens.RefreshCommand = new Command((obj) => RecarregarConteudo(null, null));

            Task.Run(async () => await CarregarConteudo());
        }

        async void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as Item;
            if (item != null)
            {
                lvItens.SelectedItem = null;
                if (item.DeveAbrirExternamente)
                {
                    Device.OpenUri(new Uri(item.UrlExterna));
                }
                else
                {
                    await ControladorDeAnuncios.VerificarEExibirAnuncioPopup();
                    await Navigation.PushAsync(new DetalhesPage(item));
                }
            }
        }

        protected async void Handle_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (_viewModel.Itens != null
                && _viewModel.Itens.Count >= AppConstants.QUANTIDADE_NOTICIAS_POR_LOTE
                && _viewModel.Itens.Count - 5 > 0
                && e.Item == _viewModel.Itens[_viewModel.Itens.Count - 5]
                && !IdsQueJaChamaramPaginacao.Contains((e.Item as Item).IdLocal)
                )
            {
                IdsQueJaChamaramPaginacao.Add((e.Item as Item).IdLocal);
                await Task.Run(() => CarregarConteudo());
            }
        }

        private async void RecarregarConteudo(object sender, System.EventArgs e)
        {
            _viewModel.Itens = new System.Collections.ObjectModel.ObservableCollection<Item>();
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
            if (!CrossConnectivity.Current.IsConnected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoAdicional.IsVisible = false;

                    //Verifica se existem Campings baixados
                    bool existemCampingsBD = CampingServices.ExistemCampingsBD();

                    FormattedString fs = new FormattedString();
                    fs.Spans.Add(new Span { Text = "Este conteúdo requer conexão com a internet.\n\n", FontAttributes = FontAttributes.Bold, FontSize = 20 });

                    if (existemCampingsBD)
                    {
                        fs.Spans.Add(new Span { Text = "Se preferir acesse o guia de campings, disponível off-line!" });
                        fs.Spans.Add(new Span
                        {
                            Text = "\nToque aqui para acessar",
                            TextColor = AppColors.COR_DESTAQUE,
                            FontAttributes = FontAttributes.Bold,
                            FontSize = Device.GetNamedSize(NamedSize.Medium
                            , typeof(Label))
                        });
                        var tap = new TapGestureRecognizer();
                        tap.Tapped += (s, e) =>
                        {
                            MessagingCenter.Send(App.Current, AppConstants.MENSAGEM_EXIBIR_BUSCA_CAMPINGS);
                            var mainPage = ((((App.Current.MainPage as RootPage).Detail as NavigationPage)).CurrentPage as MainPage);
                            mainPage.SelectedItem = null;
                            mainPage.SelectedItem = mainPage.Children[0];
                        };
                        lbMensagemAviso.GestureRecognizers.Add(tap);
                    }
                    else
                    {
                        fs.Spans.Add(new Span { Text = "Verifique sua conexão e toque para tentar novamente." });
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
                Device.BeginInvokeOnMainThread(() => { loaderConteudoAdicional.IsVisible = true; });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() => { loaderConteudoInicial.IsVisible = loaderConteudoInicial.IsRunning = loaderConteudoInicial.IsVisible = true; });
            }

            await _viewModel.Carregar(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca, TipoListagem.Noticias);


            Device.BeginInvokeOnMainThread(() =>
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
                    FormattedString fs = new FormattedString();
                    fs.Spans.Add(new Span { Text = "Nenhum item disponível.\n\n", FontAttributes = FontAttributes.Bold, FontSize = 20 });
                    fs.Spans.Add(new Span { Text = "Confira sua internet e/ou tente novamente mais tarde." });
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