using CommunityToolkit.Mvvm.Messaging;
using MaCamp.CustomControls;
using MaCamp.Services;
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

            FirstAppeared += CampingsPage_FirstAppeared;

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_ExibirBuscaCampings);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_ExibirBuscaCampings, (recipient, message) =>
            {
                cvContent.Content = new FormBuscaView();
            });

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscaRealizada, (recipient, message) =>
            {
                DBContract.UpdateKeyValue(AppConstants.Busca_InicialRealizada, Convert.ToString(true), TipoChave.ControleInterno);

                cvContent.Content = new ListagemCampingsView();
            });

            WeakReferenceMessenger.Default.Unregister<object, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados);

            WeakReferenceMessenger.Default.Register<string, string>(this, AppConstants.WeakReferenceMessenger_BuscarCampingsAtualizados, (recipient, message) =>
            {
                DBContract.UpdateKeyValue(AppConstants.Busca_InicialRealizada, Convert.ToString(true), TipoChave.ControleInterno);

                cvContent.Content = new ListagemCampingsView();
            });

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Página do Camping: ");
        }

        private void CampingsPage_FirstAppeared(object? sender, EventArgs e)
        {
            CarregarConteudo();
        }

        private void CarregarConteudo()
        {
            var buscaInicialRealizada = DBContract.GetKeyValue(AppConstants.Busca_InicialRealizada);

            if (buscaInicialRealizada != null)
            {
                cvContent.Content = new ListagemCampingsView();
            }
            else
            {
                var valorEstabelecimentos = "Campings,PontodeApoioaRV`s,CampingSelvagem/WildCamping/Bushcfaft,SemFunçãoCamping/ApoioouFechado";

                DBContract.UpdateKeyValue(AppConstants.Filtro_EstabelecimentoSelecionados, valorEstabelecimentos);

                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    var lbMensagemAviso = new Label
                    {
                        TextColor = Color.FromArgb("#414141"),
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = 20
                    };
                    var formattedString = new FormattedString();

                    formattedString.Spans.Add(new Span
                    {
                        Text = "O primeiro acesso requer conexão com a internet.\n\n",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });
                    formattedString.Spans.Add(new Span
                    {
                        Text = AppConstants.Descricao_SemInternet
                    });

                    lbMensagemAviso.FormattedText = formattedString;

                    var gestureRecognizer = new TapGestureRecognizer();

                    gestureRecognizer.Tapped += delegate
                    {
                        CarregarConteudo();
                    };

                    lbMensagemAviso.GestureRecognizers.Add(gestureRecognizer);

                    BackgroundColor = Color.FromArgb("#E4E4E4");
                    cvContent.Content = lbMensagemAviso;

                    return;
                }

                BackgroundColor = Colors.White;
                cvContent.Content = new FormBuscaView();
            }
        }
    }
}