using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Aspbrasil.Views.Detalhes
{
    public partial class DetalhesPage : ContentPage
    {
        public DetalhesPage(Item item)
        {
            InitializeComponent();
            Title = AppSettings.AppConstants.NOME_APP;

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Detalhes - " + item.Titulo);

            if (!string.IsNullOrWhiteSpace(item.URLImagemMaior))
            {
                //imgFotoItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
                imgFotoItem.HeightRequest = App.SCREEN_WIDTH * 9 / 16;

                imgFotoItem.Source = Aspbrasil.Models.Services.CampingServices.MontarUrlImagemTemporaria(item.URLImagem);
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
            else { grdFotoPrincipal.IsVisible = false; }

            lbTitulo.Text = item.Nome.ToUpper();

            //if (!string.IsNullOrWhiteSpace(item.UriKmlLocal))
            //{
            //    btAbrirMapa.BackgroundColor = Color.FromHex(App.DADOS_LOCAL.CorTopo);
            //    btAbrirMapa.IsVisible = true;
            //    btAbrirMapa.Clicked += (s, e) => AbrirMapa(item.UriKmlLocal);
            //}

            MessagingCenter.Subscribe<Application, int>(this, "ATUALIZAR_PROGRESSO_WEBVIEW", async (s, e) =>
            {
                if (e == 100)
                {
                    MessagingCenter.Unsubscribe<Application, int>(this, "ATUALIZAR_PROGRESSO_WEBVIEW");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        progress.IsVisible = false;
                    });

                    item.Visualizado = true;
                    DBContract.NewInstance().InserirOuSubstituirModelo(item);
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

            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = AppSettings.StyleCSS.style + item.Descricao.Replace("\r\n", "<br/>");
            wvDetalhes.Source = htmlSource;

        }

        public static async Task AbrirMapa(string uriArquivoKml)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    try
                    {
                        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                        if (status != PermissionStatus.Granted)
                        {
                            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                            {
                                await App.Current.MainPage.DisplayAlert("Localização necessária", "A permissão de localização será necessária para exibir o mapa", "OK");
                            }

                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                            status = results[Permission.Location];
                        }

                        if (status == PermissionStatus.Granted)
                        {
                            //await (App.Current.MainPage as RootPage).Detail.Navigation.PushAsync(new MapaPage(uriArquivoKml));
                        }
                        else if (status != PermissionStatus.Unknown)
                        {
                            await App.Current.MainPage.DisplayAlert("Localização negada", "Não é possível continuar, tente novamente.", "OK");
                        }
                    }
                    catch { }
                    break;
                case Device.iOS:
                    //await (App.Current.MainPage as RootPage).Detail.Navigation.PushAsync(new MapaPage(uriArquivoKml));
                    break;
                default:
                    break;
            }
        }



    }
}