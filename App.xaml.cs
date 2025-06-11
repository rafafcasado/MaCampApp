using System.Globalization;
using MaCamp.Dependencias;
using MaCamp.Dependencias.Permissions;
using MaCamp.Resources.Locale;
using MaCamp.Services;
using MaCamp.Utils;
using MaCamp.Views;
using MaCamp.Views.Campings;
using MaCamp.Views.Menu;

namespace MaCamp
{
    public partial class App : Application
    {
        public static int SCREEN_HEIGHT;
        public static int SCREEN_WIDTH;
        public static Location? LOCALIZACAO_USUARIO { get; set; }
        public static bool BAIXANDO_CAMPINGS { get; set; }
        public static string PATH { get; private set; } = string.Empty;

        public static event EventHandler? Resumed;

        public App()
        {
            InitializeComponent();

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;

            UserAppTheme = AppTheme.Light;

            SCREEN_HEIGHT = Convert.ToInt32(displayInfo.Height / displayInfo.Density);
            SCREEN_WIDTH = Convert.ToInt32(displayInfo.Width / displayInfo.Density);

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

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new SplashScreen(async () =>
            {
                var result = await InitializeDatabaseAsync(AppConstants.UsarPermissaoExterna);

                if (result)
                {
                    var page = new RootPage();

                    page.Appearing += async delegate
                    {
                        //OneSignalServices.RegisterIOS();
                        //new OneSignalServices(AppConstants.OnesignalAppId).InicializarOneSignal();

                        await CheckUpdateDatabaseAsync();
                        //await ShowPushNotificationAsync();
                    };

                    return page;
                }

                Environment.Exit(0);

                return new ContentPage();
            }));
        }

        public override void CloseWindow(Window window)
        {
            base.CloseWindow(window);

            BackgroundUpdater.Stop();
        }

        protected override async void OnStart()
        {
            var localizeService = await Workaround.GetServiceAsync<ILocalize>();
            var cultureInfo = localizeService.PegarCultureInfoUsuario();

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

        private static async Task<bool> InitializeDatabaseAsync(bool useExternalDirectory)
        {
            var storagePermissionService = await Workaround.GetServiceAsync<IStoragePermission>();
            var storagePermissionResult = !useExternalDirectory || await storagePermissionService.RequestExternalPermissionAsync();

            if (storagePermissionResult)
            {
                var path = useExternalDirectory ? storagePermissionService.GetExternalDirectory() : storagePermissionService.GetInternalDirectory();

                PATH = Path.Combine(path, AppConstants.NomeApp);

                if (!Directory.Exists(PATH))
                {
                    Directory.CreateDirectory(PATH);
                }

                await DBContract.InitializeAsync();

                return true;
            }

            return false;
        }

        private static async Task CheckUpdateDatabaseAsync()
        {
            var chaveData = await DBContract.GetKeyValueAsync(AppConstants.Chave_UltimaAtualizacao);

            if (chaveData == null || DateTime.TryParse(chaveData, out var data) && data < DateTime.Now.Subtract(AppConstants.Tempo_RotinaAtualizacao))
            {
                if (Current != null)
                {
                    var window = Current.Windows.FirstOrDefault();

                    if (window != null && window.Page is RootPage rootPage && rootPage.Detail is NavigationPage navigationPage)
                    {
                        await navigationPage.PushAsync(new BuscarCampings());
                    }
                }
            }
        }

        private static async Task ShowPushNotificationAsync()
        {
            var tituloPush = await DBContract.GetKeyValueAsync(AppConstants.Chave_TituloNotificacao);
            var mensagemPush = await DBContract.GetKeyValueAsync(AppConstants.Chave_MensagemNotificacao);
            var itemPush = await DBContract.GetKeyValueAsync(AppConstants.Chave_IdItemNotificacao);

            await DBContract.UpdateKeyValue(AppConstants.Chave_TituloNotificacao, null);
            await DBContract.UpdateKeyValue(AppConstants.Chave_MensagemNotificacao, null);
            await DBContract.UpdateKeyValue(AppConstants.Chave_IdItemNotificacao, null);

            if (mensagemPush != null)
            {
                await AppConstants.CurrentPage.DisplayAlert($"{tituloPush}", $"{mensagemPush}", "OK");
            }

            if (itemPush != null)
            {
                //var idLoja = itemPush.Split('_')[0];
                //var idPedido = itemPush.Split('_')[1];
                //var pedido = NetUtils.GetAsync<Pedido>($"{AppConstants.URL_API}/PedidosAPI/GetPedido?idOS={idPedido}&idLoja={idLoja}");

                //if (pedido != null)
                //{
                //    AppConstants.CurrentPage.Dispatch(async () =>
                //    {
                //        var abrir = await AppConstants.CurrentPage.DisplayAlert($"{tituloPush}", $"{mensagemPush} - Deseja visualizar agora?", "Sim", "Não");

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