using Aspbrasil.AppSettings;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.Views.Campings;
using Aspbrasil.Views.Popups;
using Plugin.Connectivity;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Aspbrasil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CampingsPage : ContentPage
    {
        public CampingsPage()
        {
            InitializeComponent();
            Title = "Campings";

            // ToolbarItems.Add(new ToolbarItem("Buscar campings", "icone_busca.png", async () =>
            //{
            //    await Navigation.PushPopupAsync(new FormBuscaPopupPage());
            //}));

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Página do Camping: ");

            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MENSAGEM_EXIBIR_BUSCA_CAMPINGS);
            MessagingCenter.Subscribe<Application>(this, AppConstants.MENSAGEM_EXIBIR_BUSCA_CAMPINGS, (r) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    cvContent.Content = new FormBuscaView();
                });
            });

            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MENSAGEM_BUSCA_REALIZADA);
            MessagingCenter.Subscribe<Application>(this, AppConstants.MENSAGEM_BUSCA_REALIZADA, (r) =>
            {
                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor("BUSCA_INICIAL_REALIZADA", "true", TipoChave.ControleInterno));
                Device.BeginInvokeOnMainThread(() =>
                {
                    cvContent.Content = new ListagemCampingsView("");
                });
            });

            CarregarConteudo();

            Task.Run(() => ObterPermissaoLocalizacao());
        }

        private void CarregarConteudo()
        {
            string buscaInicialRealizada = DBContract.NewInstance().ObterValorChave("BUSCA_INICIAL_REALIZADA");
            if (buscaInicialRealizada != null)
            {
                cvContent.Content = new ListagemCampingsView("");
            }
            else
            {
                string valorEstabelecimentos = "Campings,PontodeApoioaRV`s,CampingSelvagem/WildCamping/Bushcfaft,SemFunçãoCamping/ApoioouFechado";
                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_ESTABELECIMENTO_SELECIONADOS", Valor = valorEstabelecimentos });

                if (!CrossConnectivity.Current.IsConnected)
                {
                    Label lbMensagemAviso = new Label { TextColor = Color.FromHex("414141"), HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, Margin = 20 };
                    FormattedString fs = new FormattedString();
                    fs.Spans.Add(new Span { Text = "O primeiro acesso requer conexão com a internet.\n\n", FontAttributes = FontAttributes.Bold, FontSize = 20 });
                    fs.Spans.Add(new Span { Text = "Verifique sua conexão e toque para tentar novamente." });
                    lbMensagemAviso.FormattedText = fs;

                    TapGestureRecognizer carregarNovamente = new TapGestureRecognizer();
                    carregarNovamente.Tapped += (s, e) => CarregarConteudo();
                    lbMensagemAviso.GestureRecognizers.Add(carregarNovamente);
                    BackgroundColor = Color.FromHex("#E4E4E4");
                    cvContent.Content = lbMensagemAviso;
                    return;
                }
                BackgroundColor = Color.White;
                cvContent.Content = new FormBuscaView();
            }
        }

        private async void ObterPermissaoLocalizacao()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Localização", "Forneça a permissão de localização para poder visualizar a distância entre você e os campings", "OK"));
                    }
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location)) status = results[Permission.Location];
                }
            }

            try
            {
                var locator = CrossGeolocator.Current;
                App.LOCALIZACAO_USUARIO = await locator.GetPositionAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}