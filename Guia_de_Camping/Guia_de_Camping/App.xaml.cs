using Aspbrasil.AppSettings;
using Aspbrasil.Dependencias;
using Aspbrasil.Models;
using Aspbrasil.Models.DataAccess;
using Aspbrasil.Models.Services;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Aspbrasil
{
    public partial class App : Application
    {
        public static int SCREEN_HEIGHT;
        public static int SCREEN_WIDTH;
        public static Size ScreenPixelsSize;

        public static Position LOCALIZACAO_USUARIO { get; set; }
        public static bool EXISTEM_CAMPINGS_DISPONIVEIS { get; set; }
        public static bool BAIXANDO_CAMPINGS { get; set; } = false;

        public App()
        {
            DBContract.NewInstance();
            InitializeComponent();
            DefinirIdiomaDoApp();
            CarregarTamanhoTela();
            
            //OneSignalServices.RegisterIOS();
            new OneSignalServices(AppConstants.ONESIGNAL_APP_ID).InicializarOneSignal();



            //Task.Run(() => VerificarDownloadCampings());
            VerificarDownloadCampings();
            MainPage = new Views.Menu.RootPage();
            
            DBContract db = new DBContract();
            db.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_SERVICO_SELECIONADOS", Valor = "" });
            db.InserirOuSubstituirModelo(new ChaveValor { Chave = "FILTROS_NOME_DO_CAMPING", Valor = "" });
        }

        private async void VerificarDownloadCampings()
        {
            
            if (App.BaixarUltimaVersaoConteudo())
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool baixar = await App.Current.MainPage.DisplayAlert($"Dados atualizados disponiveis", $"Deseja baixar agora?", "Baixar", "Cancelar");

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
        ///     Define o idioma do aplicativo com base na informa��o que vem das plataformas Android e iOS. 
        /// </summary>
        private void DefinirIdiomaDoApp()
        {
            // A plataforma Windows n�o precisa da depend�ncia.
            if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS) { Aspbrasil.Resources.Locale.AppLanguage.Culture = DependencyService.Get<ILocalize>().ObterCultureInfoDoUsuario(); }
        }

        /// <summary>
        ///     Carrega o tamanho de tela do dispositivo atual e salva na vari�vel global para facilitar o acesso.
        /// </summary>
        private void CarregarTamanhoTela()
        {
            var size = Plugin.XamJam.Screen.CrossScreen.Current.Size;
            SCREEN_HEIGHT = (int)size.Height;
            SCREEN_WIDTH = (int)size.Width;
        }

        public static void ExibirNotificacaoPush()
        {
            var sqliteConnection = DBContract.NewInstance();


            string tituloPush = sqliteConnection.ObterValorChave(AppConstants.CHAVE_TITULO_NOTIFICACAO);
            string mensagemPush = sqliteConnection.ObterValorChave(AppConstants.CHAVE_MENSAGEM_NOTIFICACAO);
            string itemPush = sqliteConnection.ObterValorChave(AppConstants.CHAVE_ID_ITEM_NOTIFICACAO);

            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_TITULO_NOTIFICACAO, Valor = null });
            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_MENSAGEM_NOTIFICACAO, Valor = null });
            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_ID_ITEM_NOTIFICACAO, Valor = null });

            if (mensagemPush != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert($"{tituloPush}", $"{mensagemPush}", "OK");
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
                //        catch (Exception) { }

                //        if (pedido != null)
                //        {
                //            Device.BeginInvokeOnMainThread(async () =>
                //            {
                //                bool abrir = await App.Current.MainPage.DisplayAlert($"{tituloPush}", $"{mensagemPush} - Deseja visualizar agora?", "Sim", "N�o");
                //                if (abrir)
                //                {
                //                    await App.Current.MainPage.Navigation.PushAsync(new DetalhesPedidoPage(pedido));
                //                }
                //            });
                //        }
                //  }
            }

        }

        public static bool BaixarUltimaVersaoConteudo()
        {
            var sqliteConnection = DBContract.NewInstance();

            string dataUltimaAtualizacao = sqliteConnection.ObterValorChave(AppConstants.CHAVE_DATA_ULTIMA_ATUALIZACAO_CONTEUDO);
            System.DateTime data;
            string formato = "yyyy/MM/dd";

            if (System.DateTime.TryParseExact(dataUltimaAtualizacao, formato, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data))
            {
                //if (System.DateTime.Now.AddMinutes(-1) <= System.DateTime.Now) // Para testar a regra
                if (data.AddDays(20) <= System.DateTime.Now)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
