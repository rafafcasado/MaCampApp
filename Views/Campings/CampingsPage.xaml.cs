using CommunityToolkit.Mvvm.Messaging;
using MaCamp.CustomControls;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Views.Campings
{
    public partial class CampingsPage : SmartContentPage
    {
        public CampingsPage()
        {
            InitializeComponent();

            Title = "Campings";

            // ToolbarItems.Add(new ToolbarItem("Buscar campings", "icone_busca.png", async () =>
            //{
            //    await Navigation.PushPopupAsync(new FormBuscaPopupPage());
            //}));

            FirstAppeared += CampingsPage_FirstAppeared;

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_ExibirBuscaCampings);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_ExibirBuscaCampings, (recipient, message) =>
            {
                cvContent.Content = new FormBuscaView();
            });

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada, async (recipient, message) =>
            {
                await DBContract.UpdateKeyValueAsync(AppConstants.Busca_InicialRealizada, "true", TipoChave.ControleInterno);

                cvContent.Content = new ListagemCampingsView(string.Empty);
            });

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados, async (recipient, message) =>
            {
                await DBContract.UpdateKeyValueAsync(AppConstants.Busca_InicialRealizada, "true", TipoChave.ControleInterno);

                cvContent.Content = new ListagemCampingsView();
            });

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Página do Camping: ");
        }

        private async void CampingsPage_FirstAppeared(object? sender, EventArgs e)
        {
            await CarregarConteudoAsync();
        }

        private async Task CarregarConteudoAsync()
        {
            var buscaInicialRealizada = await DBContract.GetKeyValueAsync(AppConstants.Busca_InicialRealizada);

            if (buscaInicialRealizada != null)
            {
                cvContent.Content = new ListagemCampingsView(string.Empty);
            }
            else
            {
                var valorEstabelecimentos = "Campings,PontodeApoioaRV`s,CampingSelvagem/WildCamping/Bushcfaft,SemFunçãoCamping/ApoioouFechado";

                await DBContract.UpdateKeyValueAsync(AppConstants.Filtro_EstabelecimentoSelecionados, valorEstabelecimentos);

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

                    carregarNovamente.Tapped += async delegate
                    {
                        await CarregarConteudoAsync();
                    };

                    lbMensagemAviso.GestureRecognizers.Add(carregarNovamente);

                    BackgroundColor = Color.FromArgb("#E4E4E4");
                    cvContent.Content = lbMensagemAviso;

                    return;
                }

                BackgroundColor = Colors.White;
                cvContent.Content = new FormBuscaView();
            }

            var permissionGranted = await Workaround.CheckPermissionAsync<Permissions.LocationWhenInUse>("Localização", "Forneça a permissão de localização para poder visualizar a distância entre você e os campings");

            if (permissionGranted)
            {
                try
                {
                    App.LOCALIZACAO_USUARIO = await Geolocation.GetLastKnownLocationAsync();
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(CampingsPage), nameof(CarregarConteudoAsync), ex);
                }
            }
        }
    }
}