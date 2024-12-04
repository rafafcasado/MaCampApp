using Aspbrasil.AppSettings;
using Aspbrasil.Models.DataAccess;
using OneSignalSDK.Xamarin;
using OneSignalSDK.Xamarin.Core;
using System.Collections.Generic;

namespace Aspbrasil.Models
{
    public class OneSignalServices
    {
        private string App_Id;

        public OneSignalServices(string app_id)
        {
            App_Id = app_id;
        }

        public void InicializarOneSignal()
        {

            OneSignal.Default.Initialize(App_Id);
            OneSignal.Default.PromptForPushNotificationsWithUserResponse();

            //onesignal antigo

            //OneSignal.Current.StartInit(App_Id)
            //    .Settings(new Dictionary<string, bool>() {
            //        { IOSSettings.kOSSettingsKeyAutoPrompt, false },
            //        { IOSSettings.kOSSettingsKeyInAppLaunchURL, true } })
            //    .HandleNotificationReceived(HandlerNotificacaoRecebida)
            //    .HandleNotificationOpened(HandlerNotificacaoAberta)
            //    .InFocusDisplaying(OSInFocusDisplayOption.Notification)
            //    .EndInit();

            
        }

        // Just for iOS.
        // No effect on Android, device auto registers without prompting.
        //public static void RegisterIOS()
        //{
        //    //OneSignal.Current.RegisterForPushNotifications();
        //    OneSignal.Default.PromptForPushNotificationsWithUserResponse();
        //}

        //private void HandlerNotificacaoRecebida(OSNotification notification)
        //{
        //    ExtrairETrabalharInformacoesRecebidas(notification.payload, false);
        //}

        //private void HandlerNotificacaoAberta(OSNotificationOpenedResult result)
        //{
        //    ExtrairETrabalharInformacoesRecebidas(result.notification.payload);
        //}

        //private void ExtrairETrabalharInformacoesRecebidas(OSNotificationPayload payload, bool tentarAbrir = true)
        //{
        //    try
        //    {
        //        Dictionary<string, object> additionalData = payload.additionalData;

        //        var sqliteConnection = DBContract.NewInstance();

        //        sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_TITULO_NOTIFICACAO, Valor = payload.title });
        //        sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_MENSAGEM_NOTIFICACAO, Valor = payload.body });

        //        if (additionalData != null && additionalData.Count > 0)
        //        {
        //            if (additionalData.ContainsKey("id"))
        //            {
        //                string id = additionalData["id"].ToString();
        //                sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_ID_ITEM_NOTIFICACAO, Valor = id });
        //            }
        //        }

        //        if (payload.launchURL != null)
        //        {
        //            sqliteConnection.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_URL_NOTIFICACAO, Valor = payload.launchURL });
        //        }

        //        if (App.Current != null && tentarAbrir)
        //        {
        //            App.ExibirNotificacaoPush();
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(e.StackTrace);
        //    }
        //}
    }
}
