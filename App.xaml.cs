using System.Globalization;
using MaCamp.Dependencias;
using MaCamp.Resources.Locale;
using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.Views;
using MaCamp.Views.Menu;

namespace MaCamp
{
    public partial class App : Application
    {
        public static int SCREEN_HEIGHT;
        public static int SCREEN_WIDTH;
        public static Location? LOCALIZACAO_USUARIO { get; set; }
        public static bool BAIXANDO_CAMPINGS { get; set; }

        public static event EventHandler? Resumed;

        public App()
        {
            InitializeComponent();

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            SCREEN_HEIGHT = Convert.ToInt32(displayInfo.Height / displayInfo.Density);
            SCREEN_WIDTH = Convert.ToInt32(displayInfo.Width / displayInfo.Density);

            UserAppTheme = AppTheme.Light;
            MainPage = new SplashScreen(async () =>
            {
                var storagePermissionService = await Workaround.GetServiceAsync<IStoragePermission>();
                var storagePermissionResult = await storagePermissionService.RequestAsync();

                if (storagePermissionResult && Current != null)
                {
                    return new RootPage();
                }

                Environment.Exit(0);

                return new ContentPage();
            });

            PageAppearing += App_PageAppearing;

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var excecao = args.ExceptionObject is Exception exception ? exception : new Exception($"não foi possível converter o valor de {nameof(args)}_{nameof(args.ExceptionObject)} para {nameof(Exception)}");

                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(App), nameof(AppDomain.CurrentDomain.UnhandledException), excecao);
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(TaskScheduler), nameof(TaskScheduler.UnobservedTaskException), args.Exception);
            };
        }

        private void App_PageAppearing(object? sender, Page e)
        {
            //OneSignalServices.RegisterIOS();
            //new OneSignalServices(AppConstants.OnesignalAppId).InicializarOneSignal();
        }

        protected override async void OnStart()
        {
            //new OneSignalServices(AppConstants.OnesignalAppId).InicializarOneSignal();

            var localizeService = await Workaround.GetServiceAsync<ILocalize>();
            var cultureInfo = localizeService.PegarCultureInfoUsuario();

            await Workaround.CheckPermissionAsync<Permissions.PostNotifications>("Notificação", "Forneça a permissão para exibir os status das atualizações de dados");

            DBContract.Initialize();

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

        protected override void CleanUp()
        {
            base.CleanUp();

            BackgroundUpdater.Stop();
        }

        public static async Task ExibirNotificacaoPushAsync()
        {
            var tituloPush = DBContract.GetKeyValue(AppConstants.Chave_TituloNotificacao);
            var mensagemPush = DBContract.GetKeyValue(AppConstants.Chave_MensagemNotificacao);
            var itemPush = DBContract.GetKeyValue(AppConstants.Chave_IdItemNotificacao);

            DBContract.UpdateKeyValue(AppConstants.Chave_TituloNotificacao, null);
            DBContract.UpdateKeyValue(AppConstants.Chave_MensagemNotificacao, null);
            DBContract.UpdateKeyValue(AppConstants.Chave_IdItemNotificacao, null);

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
    }
}