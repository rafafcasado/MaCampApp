using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Models;
using MaCamp.Services;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;

namespace MaCamp.Views.Detalhes
{
    public partial class DetalhesPage : ContentPage
    {
        public DetalhesPage(Item item)
        {
            InitializeComponent();

            Title = AppConstants.NomeApp;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Detalhes - " + item.Titulo);

            if (!string.IsNullOrWhiteSpace(item.URLImagemMaior))
            {
                //imgFotoItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
                imgFotoItem.HeightRequest = Convert.ToDouble(App.SCREEN_WIDTH * 9 / 16);

                if (item.URLImagem != null)
                {
                    imgFotoItem.Source = CampingServices.MontarUrlImagemTemporaria(item.URLImagem);
                }

                //List<Arquivo> arquivosGaleria = db.BuscarArquivosGaleria(item.IdItem);
                //if (arquivosGaleria.Count > 0)
                //{
                //    //O id do item será obtido no clique através do ClassID do Grid
                //    grdFotoPrincipal.ClassId = idItem;
                //    //imIconeGaleria.Source = ImageSource.FromResource("Keer.Resources.icone_galeria.jpg");
                //    imIconeGaleria.Source = "icone_galeria.png";
                //    slMaisFotos.IsVisible = true;

                //    var tapGesture = new TapGestureRecognizer();
                //    tapGesture.Tapped += OnMaisFotosTapped;
                //    grdFotoPrincipal.GestureRecognizers.Add(tapGesture);
                //}
            }
            else
            {
                grdFotoPrincipal.IsVisible = false;
            }

            lbTitulo.Text = item.Nome?.ToUpper() ?? string.Empty;

            //if (!string.IsNullOrWhiteSpace(item.UriKmlLocal))
            //{
            //    btAbrirMapa.BackgroundColor = Color.FromHex(App.DADOS_LOCAL.CorTopo);
            //    btAbrirMapa.IsVisible = true;
            //    btAbrirMapa.Clicked += (s, e) => AbrirMapa(item.UriKmlLocal);
            //}

            ConfigurarToolbar(item);

            WeakReferenceMessenger.Default.Register<object, string>(this, AppConstants.Atualizar_ProgressoWebView, (recipient, message) =>
            {
                if (message is int value && value == 100)
                {
                    WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.Atualizar_ProgressoWebView);

                    progress.IsVisible = false;
                    item.Visualizado = true;

                    DBContract.InserirOuSubstituirModelo(item);
                }
            });

            //var htmlSource = new HtmlWebViewSource();
            //htmlSource.Html = @"<style type='text/css'>" + App.DADOS_LOCAL.CssConteudo + "</style>" +
            //    System.Net.WebUtility.HtmlDecode(item.ConteudoHtml) +
            //    @"<br/><br/><br/>";

            //+
            //   @"<div class='rodapeInterna'>
            //            <ul>
            //               <li><img src='icones/icone_telefone_rodape.jpg' alt='' /><a href='tel:1144144700'>+55 11 4414-4700</a></li>
            //            </ul>
            //      </div>"

            //htmlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            //wvDetalhes.Source = item.Descricao;

            //lbDescricao.Text = item.Descricao;
            var htmlSource = new HtmlWebViewSource
            {
                Html = StyleCSS.style + item.Descricao?.Replace("\r\n", "<br/>")
            };

            wvDetalhes.Source = htmlSource;
            wvDetalhes.Loaded += WvDetalhes_Loaded;
            wvDetalhes.Navigating += WvDetalhes_Navigating;
        }

        private void ConfigurarToolbar(Item item)
        {
            ToolbarItems.Clear();

            if (StorageHelper.IsFavoriteItem(item.Id))
            {
                var imagem = "icone_favoritos_on.png";

                ToolbarItems.Add(new ToolbarItem("Remover Favorito", imagem, () =>
                {
                    item.Favoritado = false;

                    StorageHelper.AddOrUpdateItem(item);
                    DBContract.InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);

                    WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_AtualizarListagemFavoritos);
                }));
            }
            else
            {
                var imagem = "icone_favoritos_off.png";

                ToolbarItems.Add(new ToolbarItem("Favoritar", imagem, () =>
                {
                    item.Favoritado = true;

                    StorageHelper.AddOrUpdateItem(item);
                    DBContract.InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);

                    WeakReferenceMessenger.Default.Send(string.Empty, AppConstants.WeakReferenceMessenger_AtualizarListagemFavoritos);
                }));
            }
        }

        private void WvDetalhes_Loaded(object? sender, EventArgs e)
        {
            progress.IsVisible = false;
            progress.IsRunning = false;
        }

        private async void WvDetalhes_Navigating(object? sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith("http"))
            {
                e.Cancel = true;

                await Launcher.OpenAsync(e.Url);
            }
        }

        public static async Task AbrirMapa(string uriArquivoKml)
        {
            var permissionGranted = await Workaround.CheckPermission<Permissions.LocationWhenInUse>("Localização", "A permissão de localização será necessária para exibir o mapa");

            if (permissionGranted)
            {
                //await (currentPage as RootPage).Detail.Navigation.PushAsync(new MapaPage(uriArquivoKml));
            }
        }
    }
}