using System.Globalization;
using MaCamp.Dependencias;
using MaCamp.Models;
using MaCamp.Resources.Locale;
using MaCamp.Services;
using MaCamp.Services.DataAccess;
using MaCamp.Utils;
using MaCamp.Views;
using MaCamp.Views.Menu;

namespace MaCamp
{
    public partial class App : IApplication
    {
        public static int SCREEN_HEIGHT;
        public static int SCREEN_WIDTH;
        public static Size ScreenPixelsSize;
        public static Location? LOCALIZACAO_USUARIO { get; set; }
        public static bool EXISTEM_CAMPINGS_DISPONIVEIS { get; set; }
        public static bool BAIXANDO_CAMPINGS { get; set; }

        public static event EventHandler? Resumed;

        public App()
        {
            InitializeComponent();

            CarregarTamanhoTela();

            UserAppTheme = AppTheme.Light;

            //OneSignalServices.RegisterIOS();
            //new OneSignalServices(AppConstants.OnesignalAppId).InicializarOneSignal();

            //VerificarDownloadCampings();
        }

        private async void VerificarDownloadCampings()
        {
            if (BaixarUltimaVersaoConteudo())
            {
                var baixar = await AppConstants.CurrentPage.DisplayAlert("Dados atualizados disponiveis", "Deseja baixar agora?", "Baixar", "Cancelar");

                if (baixar)
                {
                    await CampingServices.BaixarCampings(true);
                }
            }
            else
            {
                await CampingServices.BaixarCampings(false);
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashScreen(async () =>
            {
                var storagePermissionService = await Workaround.GetServiceAsync<IStoragePermission>();
                var storagePermissionResult = await storagePermissionService.Request();

                if (storagePermissionResult && Current != null)
                {
                    return new RootPage();
                }

                return new ContentPage();
            }));
        }

        protected override async void OnStart()
        {
            //new OneSignalServices(AppConstants.OnesignalAppId).InicializarOneSignal();

            var localizeService = await Workaround.GetServiceAsync<ILocalize>();
            var cultureInfo = localizeService.PegarCultureInfoUsuario();

            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            await Permissions.RequestAsync<Permissions.PostNotifications>();

            AppLanguage.Culture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        protected override void OnResume()
        {
            base.OnResume();

            Resumed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Carrega o tamanho de tela do dispositivo atual e salva na variável global para facilitar o acesso.
        /// </summary>
        private void CarregarTamanhoTela()
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            SCREEN_HEIGHT = Convert.ToInt32(displayInfo.Height / displayInfo.Density);
            SCREEN_WIDTH = Convert.ToInt32(displayInfo.Width / displayInfo.Density);
        }

        public static async void ExibirNotificacaoPush()
        {
            var tituloPush = DBContract.ObterValorChave(AppConstants.Chave_TituloNotificacao);
            var mensagemPush = DBContract.ObterValorChave(AppConstants.Chave_MensagemNotificacao);
            var itemPush = DBContract.ObterValorChave(AppConstants.Chave_IdItemNotificacao);

            DBContract.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Chave_TituloNotificacao,
                Valor = null
            });

            DBContract.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Chave_MensagemNotificacao,
                Valor = null
            });

            DBContract.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.Chave_IdItemNotificacao,
                Valor = null
            });

            if (mensagemPush != null)
            {
                await AppConstants.CurrentPage.DisplayAlert($"{tituloPush}", $"{mensagemPush}", "OK");
            }

            if (itemPush != null)
            {
                //    string idLoja = itemPush.Split('_')[0];
                //    string idPedido = itemPush.Split('_')[1];

                //var pedido = NetUtils.GetAsync<Pedido>($"{AppConstants.URL_API}/PedidosAPI/GetPedido?idOS={idPedido}&idLoja={idLoja}");

                //if (pedido != null)
                //{
                //    Current?.Dispatcher.Dispatch(async () =>
                //    {
                //        bool abrir = await AppConstants.CurrentPage.DisplayAlert($"{tituloPush}", $"{mensagemPush} - Deseja visualizar agora?", "Sim", "Não");
                //        if (abrir)
                //        {
                //            await AppConstants.CurrentPage.Navigation.PushAsync(new DetalhesPedidoPage(pedido));
                //        }
                //    });
                //}
            }
        }

        public static bool BaixarUltimaVersaoConteudo()
        {
            var dataUltimaAtualizacao = DBContract.ObterValorChave(AppConstants.Chave_DataUltimaAtualizacaoConteudo);
            var formato = "yyyy/MM/dd";

            if (DateTime.TryParseExact(dataUltimaAtualizacao, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))

            //if (System.DateTime.Now.AddMinutes(-1) <= System.DateTime.Now) // Para testar a regra
            {
                if (data.AddDays(20) <= DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }
    }
}