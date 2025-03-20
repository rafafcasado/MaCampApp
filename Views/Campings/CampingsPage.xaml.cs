using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Models;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Campings
{
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

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Página do Camping: ");

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_ExibirBuscaCampings);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_ExibirBuscaCampings, (recipient, message) =>
            {
                cvContent.Content = new FormBuscaView();
            });

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada, (recipient, message) =>
            {
                DBContract.InserirOuSubstituirModelo(new ChaveValor(AppConstants.Busca_InicialRealizada, "true", TipoChave.ControleInterno));

                cvContent.Content = new ListagemCampingsView(string.Empty);
            });

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados, (recipient, message) =>
            {
                DBContract.InserirOuSubstituirModelo(new ChaveValor(AppConstants.Busca_InicialRealizada, "true", TipoChave.ControleInterno));

                cvContent.Content = new ListagemCampingsView();
            });

            CarregarConteudo();
            ObterPermissaoLocalizacao();
        }

        private void CarregarConteudo()
        {
            var buscaInicialRealizada = DBContract.ObterValorChave(AppConstants.Busca_InicialRealizada);

            if (buscaInicialRealizada != null)
            {
                cvContent.Content = new ListagemCampingsView(string.Empty);
            }
            else
            {
                var valorEstabelecimentos = "Campings,PontodeApoioaRV`s,CampingSelvagem/WildCamping/Bushcfaft,SemFunçãoCamping/ApoioouFechado";

                DBContract.InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = AppConstants.Filtro_EstabelecimentoSelecionados,
                    Valor = valorEstabelecimentos
                });

                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    var lbMensagemAviso = new Label
                    {
                        TextColor = Color.FromArgb("#414141"),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = 20
                    };
                    var fs = new FormattedString();

                    fs.Spans.Add(new Span
                    {
                        Text = "O primeiro acesso requer conexão com a internet.\n\n",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });
                    fs.Spans.Add(new Span
                    {
                        Text = AppConstants.Descricao_SemInternet
                    });

                    lbMensagemAviso.FormattedText = fs;

                    var carregarNovamente = new TapGestureRecognizer();

                    carregarNovamente.Tapped += (s, e) => CarregarConteudo();

                    lbMensagemAviso.GestureRecognizers.Add(carregarNovamente);

                    BackgroundColor = Color.FromArgb("#E4E4E4");
                    cvContent.Content = lbMensagemAviso;

                    return;
                }

                BackgroundColor = Colors.White;
                cvContent.Content = new FormBuscaView();
            }
        }

        private async void ObterPermissaoLocalizacao()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                    if (status != PermissionStatus.Granted)
                    {
                        var shouldShowRationale = Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>();

                        if (shouldShowRationale)
                        {
                            await AppConstants.CurrentPage.DisplayAlert("Localização", "Forneça a permissão de localização para poder visualizar a distância entre você e os campings", "OK");
                        }

                        await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    }
                });
            }

            try
            {
                App.LOCALIZACAO_USUARIO = await Geolocation.GetLastKnownLocationAsync();
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(CampingsPage), nameof(ObterPermissaoLocalizacao), ex);
            }
        }
    }
}