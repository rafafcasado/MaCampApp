﻿using MaCamp.AppSettings;
using MaCamp.Models;
using MaCamp.Models.DataAccess;

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
            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MensagemExibirBuscaCampings);

            MessagingCenter.Subscribe<Application>(this, AppConstants.MensagemExibirBuscaCampings, (r) =>
            {
                Dispatcher.Dispatch(() =>
                {
                    cvContent.Content = new FormBuscaView();
                });
            });

            MessagingCenter.Unsubscribe<Application>(this, AppConstants.MensagemBuscaRealizada);

            MessagingCenter.Subscribe<Application>(this, AppConstants.MensagemBuscaRealizada, (r) =>
            {
                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor("BUSCA_INICIAL_REALIZADA", "true", TipoChave.ControleInterno));

                Dispatcher.Dispatch(() =>
                {
                    cvContent.Content = new ListagemCampingsView("");
                });
            });

            CarregarConteudo();
            Task.Run(ObterPermissaoLocalizacao);
        }

        private void CarregarConteudo()
        {
            var buscaInicialRealizada = DBContract.NewInstance().ObterValorChave("BUSCA_INICIAL_REALIZADA");

            if (buscaInicialRealizada != null)
            {
                cvContent.Content = new ListagemCampingsView("");
            }
            else
            {
                var valorEstabelecimentos = "Campings,PontodeApoioaRV`s,CampingSelvagem/WildCamping/Bushcfaft,SemFunçãoCamping/ApoioouFechado";

                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = "FILTROS_ESTABELECIMENTO_SELECIONADOS",
                    Valor = valorEstabelecimentos
                });

                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    var lbMensagemAviso = new Label
                    {
                        TextColor = Color.FromArgb("414141"), HorizontalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center, Margin = 20
                    };

                    var fs = new FormattedString();

                    fs.Spans.Add(new Span
                    {
                        Text = "O primeiro acesso requer conexão com a internet.\n\n", FontAttributes = FontAttributes.Bold,
                        FontSize = 20
                    });

                    fs.Spans.Add(new Span
                    {
                        Text = "Verifique sua conexão e toque para tentar novamente."
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
                            await Dispatcher.DispatchAsync(async () => await AppConstants.CurrentPage.DisplayAlert("Localização", "Forneça a permissão de localização para poder visualizar a distância entre você e os campings", "OK"));
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}