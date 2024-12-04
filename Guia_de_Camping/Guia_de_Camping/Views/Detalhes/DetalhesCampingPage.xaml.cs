using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Plugin.ExternalMaps;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Clipboard;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Reflection;

namespace Aspbrasil.Views.Detalhes
{
    public partial class DetalhesCampingPage : ContentPage
    {
        private Item ItemAtual;


       public DetalhesCampingPage(Item item)
        {
            InitializeComponent();
            Title = AppSettings.AppConstants.NOME_APP;
            NavigationPage.SetBackButtonTitle(this, string.Empty);

            ItemAtual = item;
            BindingContext = ItemAtual;


            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Detalhes - " + item.Nome);
            if (!string.IsNullOrWhiteSpace(item.LinkUltimaFoto))
            {
                imgFotoItem.DownsampleWidth = App.SCREEN_WIDTH * 1.5;
                imgFotoItem.HeightRequest = App.SCREEN_WIDTH * 9 / 16;

                //imgFotoItem.Source = item.LinkUltimaFoto;
                imgFotoItem.Source = Aspbrasil.Models.Services.CampingServices.MontarUrlImagemTemporaria(item.LinkUltimaFoto);

                //imgFotoItem.Source = "https://img.freepik.com/fotos-premium/cachorro-andando-na-rua_41691-381.jpg?w=1380";

                if (!string.IsNullOrWhiteSpace(item.LinksFotos))
                {
                    //O id do item será obtido no clique através do ClassID do Grid
                    /*grdFotoPrincipal.ClassId = item.LinksFotos;*/
                    //imIconeGaleria.Source = ImageSource.FromResource("Keer.Resources.icone_galeria.jpg");
                    imIconeGaleria.Source = "icone_galeria.png";
                    slMaisFotos.IsVisible = true;

                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += async (s, e) =>
                    {
                        await Navigation.PushAsync(new ListagemFotosPage(item.LinksFotos.Split('|')));
                    };
                    grdFotoPrincipal.GestureRecognizers.Add(tapGesture);
                }
            }
            else { grdFotoPrincipal.IsVisible = false; }

            lbTitulo.Text = item.Nome.ToUpper();
            Task.Delay(500).ContinueWith((r) =>
            {
                string descricao = Encoding.UTF8.GetString(Convert.FromBase64String(item.Descricao));
                Device.BeginInvokeOnMainThread(() =>
                {
                    //lbDescricao.Text = descricao;
                    lbDescricao.Text = descricao.Replace("\r\n", "<br/>");
                    cvTipo.Content = new TipoEstabelecimentoView(item.Identificadores);
                    cvComodidades.Content = new ComodidadesView(item.Identificadores);
                });
            });
            ConfigurarToolbar(item);

            var abrirMapa = new TapGestureRecognizer();
            if (item.Latitude == 0 && item.Longitude == 0)
            {
                slCoordenadas.IsVisible = false;
                abrirMapa.Tapped += (s, e) => { AbrirEndereco(s, null); };
            }
            else
            {
                abrirMapa.Tapped += (s, e) => { AbrirLatitudeLongitude(s, null); };
            }
            slEndereco.GestureRecognizers.Add(abrirMapa);


            var abrirSite = new TapGestureRecognizer();
            {
                abrirSite.Tapped += (s, e) => { AbrirURI2(s, ItemAtual.Site); };
            }
            slSite.GestureRecognizers.Add(abrirSite);


            var abrirPreco = new TapGestureRecognizer();
            {
                abrirPreco.Tapped += (s, e) => { AbrirURI2(s, ItemAtual.LinkPrecos); };
            }
            slPreco.GestureRecognizers.Add(abrirPreco);

            var abrirFacebook = new TapGestureRecognizer();
            {
                abrirFacebook.Tapped += (s, e) => { AbrirURI2(s, ItemAtual.Facebook); };
            }
            slFacebook.GestureRecognizers.Add(abrirFacebook);

            var abrirInstragram = new TapGestureRecognizer();
            {
                abrirInstragram.Tapped += (s, e) => { AbrirURI2(s, ItemAtual.Instagram); };
            }
            slInstagram.GestureRecognizers.Add(abrirInstragram);

            var abrirYoutube = new TapGestureRecognizer();
            {
                abrirYoutube.Tapped += (s, e) => { AbrirURI2(s, ItemAtual.Youtube); };
            }
            slYoutube.GestureRecognizers.Add(abrirYoutube);

            var abrirEmail = new TapGestureRecognizer();
            {
                abrirEmail.Tapped += (s, e) => { AbrirMail2(s, ItemAtual.Email); };
            }
            slEmail.GestureRecognizers.Add(abrirEmail);



            var abrirTelefone = new TapGestureRecognizer();
            {
                abrirTelefone.Tapped += (s, e) => { AbrirTelefone2(s, ItemAtual.Telefone); };
            }
            slTelefone1.GestureRecognizers.Add(abrirTelefone);

            var abrirTelefone2 = new TapGestureRecognizer();
            {
                abrirTelefone2.Tapped += (s, e) => { AbrirTelefone2(s, ItemAtual.Telefone2); };
            }
            slTelefone2.GestureRecognizers.Add(abrirTelefone2);
            
            var abrirTelefone3 = new TapGestureRecognizer();
            {
                abrirTelefone3.Tapped += (s, e) => { AbrirTelefone2(s, ItemAtual.Telefone3); };
            }
            slTelefone3.GestureRecognizers.Add(abrirTelefone3);
            
            var abrirTelefone4 = new TapGestureRecognizer();
            {
                abrirTelefone4.Tapped += (s, e) => { AbrirTelefone2(s, ItemAtual.Telefone4); };
            }
            slTelefone4.GestureRecognizers.Add(abrirTelefone4);






        }

        void ConfigurarToolbar(Item item)
        {
            ToolbarItems.Clear();

            if (!item.Favoritado)
            {
                string imagem = "icone_favoritos_off.png";
                ToolbarItems.Add(new ToolbarItem("Favoritar", imagem, () =>
                {
                    item.Favoritado = true;
                    DBContract.NewInstance().InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);
                    MessagingCenter.Send<Application>(App.Current, AppConstants.MENSAGEM_ATUALIZAR_LISTAGEM_FAVORITOS);
                }));
            }
            else
            {
                string imagem = "icone_favoritos_on.png";
                ToolbarItems.Add(new ToolbarItem("Remover Favorito", imagem, () =>
                {
                    item.Favoritado = false;
                    DBContract.NewInstance().InserirOuSubstituirModelo(item);
                    ConfigurarToolbar(item);
                    MessagingCenter.Send<Application>(App.Current, AppConstants.MENSAGEM_ATUALIZAR_LISTAGEM_FAVORITOS);
                }));
            }

            ToolbarItems.Add(new ToolbarItem("Compartilhar", "share.png", async () =>
            {
                if (ItemAtual != null)
                {
                    await Plugin.Share.CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Text = ItemAtual.Nome, Url = ItemAtual.UrlExterna }, new Plugin.Share.Abstractions.ShareOptions { ChooserTitle = "Selecione" });
                }
            }));

        }

        //async void OnMaisFotosTapped(object sender, EventArgs e)
        //{
        //    string url = (sender as Grid).ClassId;
        //    await Navigation.PushModalAsync(new VisualizacaoFotoPage(url));
        //}

        public static async Task AbrirMapa(string uriArquivoKml)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    try
                    {
                        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                        if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                        {
                            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                            {
                                await App.Current.MainPage.DisplayAlert("Localização necessária", "A permissão de localização será necessária para exibir o mapa", "OK");
                            }

                            var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                            status = results[Permission.Location];
                        }

                        if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                        {
                            //await (App.Current.MainPage as RootPage).Detail.Navigation.PushAsync(new MapaPage(uriArquivoKml));
                        }
                        else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
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

        private async void AbrirEndereco(object sender, TappedEventArgs e)
        {
            var success = await CrossExternalMaps.Current.NavigateTo(ItemAtual.Nome, ItemAtual.Endereco, ItemAtual.Cidade, ItemAtual.Estado, string.Empty, ItemAtual.Pais, string.Empty);
        }

        private async void AbrirLatitudeLongitude(object sender, TappedEventArgs e)
        {
            var success = await CrossExternalMaps.Current.NavigateTo(ItemAtual.Nome, ItemAtual.Latitude.Value, ItemAtual.Longitude.Value);
        }

        private void AbrirURI(object sender, TappedEventArgs e)
        {
            Launcher.TryOpenAsync(new Uri(e.Parameter.ToString()));
        }

        private void AbrirURI2(object sender, string url)
        {
            Launcher.TryOpenAsync(new Uri(url));
        }

        private void AbrirMail(object sender, TappedEventArgs e)
        {
            Launcher.TryOpenAsync(new Uri("mailto:" + e.Parameter.ToString()));
        }

        private void AbrirMail2(object sender, string email)
        {
            Launcher.TryOpenAsync(new Uri("mailto:" + email));
        }

        private void AbrirTelefone(object sender, TappedEventArgs e)
        {
            Launcher.TryOpenAsync(new Uri("tel:" + e.Parameter.ToString().Replace(".", "").Replace("(", "").Replace(")", "")));
        }

        private void AbrirTelefone2(object sender, string telefone)
        {
            Launcher.TryOpenAsync(new Uri("tel:" + telefone.ToString().Replace(".", "").Replace("(", "").Replace(")", "")));
        }

        private async void AbrirTelaColaboracao(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FormularioColaboracaoCampingPage(ItemAtual.Nome, ItemAtual.IdCamping));
        }

        private async void CopiarCoordenadas(object sender, EventArgs e)
        {            
            CrossClipboard.Current.SetText(ItemAtual.LatitudeLongitude);
            await DisplayAlert("Coordenadas Copiadas", "Basta colar essa informação no seu aplicativo de mapas favorito.", "Ok");
        }
        protected void GoToSO(object sender, EventArgs e)
        {
            Launcher.TryOpenAsync(new Uri("https://macamp.com.br/guia/termos-de-uso/"));
        }
    }
}