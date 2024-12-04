using System.Globalization;
using MaCamp.AppSettings;
using MaCamp.Dependencias;
using MaCamp.Models;
using MaCamp.Models.DataAccess;
using MaCamp.Models.Services;

namespace MaCamp
{
    public partial class App : IApplication
    {
        public static int SCREEN_HEIGHT;
        public static int SCREEN_WIDTH;
        public static Size ScreenPixelsSize;
        public static Location? LOCALIZACAO_USUARIO { get; set; }
        public static bool EXISTEM_CAMPINGS_DISPONIVEIS { get; set; }
        public static bool BAIXANDO_CAMPINGS { get; set; } = false;

        public App()
        {
            InitializeComponent();

            CarregarTamanhoTela();

            var culture = new CultureInfo("pt-BR");

            UserAppTheme = AppTheme.Light;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            //OneSignalServices.RegisterIOS();
            //new OneSignalServices(AppConstants.OnesignalAppId).InicializarOneSignal();

            //Task.Run(() => VerificarDownloadCampings());
            //VerificarDownloadCampings();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new Views.Menu.RootPage());
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            var sqlite = Handler?.MauiContext?.Services.GetService<ISQLite>();

            if (sqlite != null)
            {
                DBContract.SqlConnection = sqlite.ObterConexao();

                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = "FILTROS_SERVICO_SELECIONADOS",
                    Valor = ""
                });
                DBContract.NewInstance().InserirOuSubstituirModelo(new ChaveValor
                {
                    Chave = "FILTROS_NOME_DO_CAMPING",
                    Valor = ""
                });
            }

            // A plataforma Windows não precisa da depend�ncia.
            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var localize = Handler?.MauiContext?.Services.GetService<ILocalize>();

                if (localize != null)
                {
                    MaCamp.Resources.Locale.AppLanguage.Culture = localize.ObterCultureInfoDoUsuario();
                }
            }
        }

        private async void VerificarDownloadCampings()
        {
            if (BaixarUltimaVersaoConteudo())
            {
                await AppConstants.CurrentPage.Dispatcher.DispatchAsync(async () =>
                {
                    var baixar = await AppConstants.CurrentPage.DisplayAlert("Dados atualizados disponiveis", "Deseja baixar agora?", "Baixar", "Cancelar");

                    if (baixar)
                    {
                        await Task.Run(() => CampingServices.BaixarCampings(true));
                    }
                });
            }

            await CampingServices.BaixarCampings();
        }

        protected override void OnStart()
        {
            //Task.Run(() => new OneSignalServices(AppConstants.ONESIGNAL_APP_ID).InicializarOneSignal());
        }

        /// <summary>
        ///     Carrega o tamanho de tela do dispositivo atual e salva na vari�vel global para facilitar o acesso.
        /// </summary>
        private void CarregarTamanhoTela()
        {
            if (GetValue(Window.HeightProperty) is int height)
            {
                SCREEN_HEIGHT = height;
            }

            if (GetValue(Window.WidthProperty) is int width)
            {
                SCREEN_WIDTH = width;
            }
        }

        public static async void ExibirNotificacaoPush()
        {
            var sqliteConnection = DBContract.NewInstance();
            var tituloPush = sqliteConnection.ObterValorChave(AppConstants.ChaveTituloNotificacao);
            var mensagemPush = sqliteConnection.ObterValorChave(AppConstants.ChaveMensagemNotificacao);
            var itemPush = sqliteConnection.ObterValorChave(AppConstants.ChaveIdItemNotificacao);

            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.ChaveTituloNotificacao,
                Valor = null
            });

            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.ChaveMensagemNotificacao,
                Valor = null
            });

            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor
            {
                Chave = AppConstants.ChaveIdItemNotificacao,
                Valor = null
            });

            if (mensagemPush != null)
            {
                await AppConstants.CurrentPage.Dispatcher.DispatchAsync(async () =>
                {
                    await AppConstants.CurrentPage.DisplayAlert($"{tituloPush}", $"{mensagemPush}", "OK");
                });
            }

            if (itemPush != null)
            {
                //    string idLoja = itemPush.Split('_')[0];
                //    string idPedido = itemPush.Split('_')[1];

                //    Pedido pedido = null;
                //    using (var client = new HttpClient())
                //    {
                //        try
                //        {
                //            string url = $"{AppConstants.URL_API}/PedidosAPI/GetPedido?idOS={idPedido}&idLoja={idLoja}";
                //            string jsonPedido = await client.GetStringAsync(url);

                //            pedido = JsonConvert.DeserializeObject<Pedido>(jsonPedido);
                //        }
                //        catch (Exception ex)
                //        {
                //            Console.WriteLine(ex.Message);
                //        }

                //        if (pedido != null)
                //        {
                //            Current.Dispatcher.Dispatch(async () =>
                //            {
                //                bool abrir = await AppConstants.CurrentPage.DisplayAlert($"{tituloPush}", $"{mensagemPush} - Deseja visualizar agora?", "Sim", "N�o");
                //                if (abrir)
                //                {
                //                    await AppConstants.CurrentPage.Navigation.PushAsync(new DetalhesPedidoPage(pedido));
                //                }
                //            });
                //        }
                //  }
            }
        }

        public static bool BaixarUltimaVersaoConteudo()
        {
            var sqliteConnection = DBContract.NewInstance();
            var dataUltimaAtualizacao = sqliteConnection.ObterValorChave(AppConstants.ChaveDataUltimaAtualizacaoConteudo);
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