﻿using System.Collections.ObjectModel;
using MaCamp.CustomControls;
using MaCamp.Models;
using MaCamp.Models.Anuncios;
using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.ViewModels;
using MaCamp.Views.CustomViews;
using MaCamp.Views.Detalhes;
using MaCamp.Views.Menu;

namespace MaCamp.Views.Listagens
{
    public partial class ListagemItensOnlineView : SmartContentView
    {
        private ListagemInfinitaViewModel ViewModel { get; }
        private int PaginaAtual;
        private List<int> IdsQueJaChamaramPaginacao { get; set; }
        private string EndpointListagem { get; }
        private string? Tag { get; }
        private string? ParametrosBusca { get; }

        public ListagemItensOnlineView(string endpointListagem, string? tag = null, string? parametrosBusca = null)
        {
            InitializeComponent();

            NavigationPage.SetBackButtonTitle(this, string.Empty);

            ViewModel = new ListagemInfinitaViewModel();
            IdsQueJaChamaramPaginacao = new List<int>();
            EndpointListagem = endpointListagem;
            Tag = tag;
            ParametrosBusca = parametrosBusca;

            FirstAppeared += ListagemItensOnlineView_FirstAppeared;

            cvItens.ItemTemplate = new ItemDataTemplateSelector();
        }

        private async void ListagemItensOnlineView_FirstAppeared(object? sender, EventArgs e)
        {
            await CarregarConteudoAsync();
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
                    await ControladorDeAnuncios.VerificarEExibirAnuncioPopupAsync();
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
                    var temMaisItens = ViewModel.Itens.Count >= AppConstants.QuantidadeNoticiasPorLote;
                    var contemItemLocal = !IdsQueJaChamaramPaginacao.Contains(item.IdLocal);

                    if (temMaisItens && ViewModel.Itens.Count - 5 > 0 && item == ViewModel.Itens[^5] && contemItemLocal)
                    {
                        IdsQueJaChamaramPaginacao.Add(item.IdLocal);

                        await CarregarConteudoAsync();
                    }
                }
            }
        }

        private async void Handle_Refreshing(object? sender, EventArgs? e)
        {
            ViewModel.Itens = new ObservableCollection<Item>();
            lbMensagemAviso.IsVisible = false;

            if (sender != null)
            {
                loaderConteudoInicial.IsVisible = true;
            }

            PaginaAtual = 0;
            cvItens.ItemsSource = null;
            IdsQueJaChamaramPaginacao = new List<int>();

            await CarregarConteudoAsync();
        }

        private async Task CarregarConteudoAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                loaderConteudoInicial.IsVisible = false;
                loaderConteudoAdicional.IsVisible = false;

                var formattedString = new FormattedString();
                var gestureRecognizer = new TapGestureRecognizer();
                //Verifica se existem Campings baixados
                var existemCampings = await CampingServices.ExistemCampingsAsync();

                formattedString.Spans.Add(new Span
                {
                    Text = $"{AppConstants.Titulo_SemInternet}.\n\n",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20
                });

                if (existemCampings)
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = "Se preferir acesse o guia de campings, disponível off-line!"
                    });
                    formattedString.Spans.Add(new Span
                    {
                        Text = "\nToque aqui para acessar",
                        TextColor = AppColors.CorDestaque,
                        FontAttributes = FontAttributes.Bold
                    });

                    gestureRecognizer.Tapped += async delegate
                    {
                        if (AppConstants.CurrentPage is RootPage rootPage && rootPage.Detail is NavigationPage navigationPage && navigationPage.CurrentPage is MainPage mainPage)
                        {
                            await navigationPage.PushAsync(mainPage.Children[0]);
                        }
                    };

                    lbMensagemAviso.GestureRecognizers.Add(gestureRecognizer);
                }
                else
                {
                    formattedString.Spans.Add(new Span
                    {
                        Text = AppConstants.Descricao_SemInternet
                    });

                    gestureRecognizer.Tapped += Handle_Refreshing;
                }

                lbMensagemAviso.FormattedText = formattedString;
                lbMensagemAviso.IsVisible = true;

                lbMensagemAviso.GestureRecognizers.Add(gestureRecognizer);

                return;
            }

            if (PaginaAtual > 0)
            {
                loaderConteudoAdicional.IsVisible = true;
            }
            else
            {
                loaderConteudoInicial.IsVisible = true;
            }

            await Workaround.TaskWorkAsync(async () =>
            {
                await ViewModel.CarregarAsync(EndpointListagem, ++PaginaAtual, Tag, ParametrosBusca);
            });

            loaderConteudoInicial.IsVisible = false;
            loaderConteudoAdicional.IsVisible = false;
            rvItens.IsRefreshing = false;

            if (ViewModel.Itens.Count > 0)
            {
                lbMensagemAviso.IsVisible = false;
                cvItens.ItemsSource = ViewModel.Itens;
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

                var gestureRecognizer = new TapGestureRecognizer();

                gestureRecognizer.Tapped += Handle_Refreshing;

                lbMensagemAviso.GestureRecognizers.Add(gestureRecognizer);
            }
        }
    }
}