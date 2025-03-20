namespace MaCamp.Services
{
    public class OneSignalServices
    {
        private string App_Id { get; }

        public OneSignalServices(string app_id)
        {
            App_Id = app_id;
        }

        public void InicializarOneSignal()
        {
            //OneSignal.Initialize(App_Id);

            //OneSignal.PromptForPushNotificationsWithUserResponse();

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
        //    OneSignal.PromptForPushNotificationsWithUserResponse();
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

        //        DBContract.InserirOuSubstituirModelo(new ChaveValor
        //        { 
        //            Chave = AppConstants.CHAVE_TITULO_NOTIFICACAO,
        //            Valor = payload.title
        //        });
        //        DBContract.InserirOuSubstituirModelo(new ChaveValor
        //        {
        //            Chave = AppConstants.CHAVE_MENSAGEM_NOTIFICACAO,
        //            Valor = payload.body
        //        });

        //        if (additionalData != null && additionalData.Count > 0)
        //        {
        //            if (additionalData.ContainsKey("id"))
        //            {
        //                string id = additionalData["id"].ToString();
        //                DBContract.InserirOuSubstituirModelo(new ChaveValor { Chave = AppConstants.CHAVE_ID_ITEM_NOTIFICACAO, Valor = id });
        //            }
        //        }

        //        if (payload.launchURL != null)
        //        {
        //            DBContract.InserirOuSubstituirModelo(new ChaveValor
        //            {
        //                Chave = AppConstants.CHAVE_URL_NOTIFICACAO,
        //                Valor = payload.launchURL
        //            });
        //        }

        //        if (tentarAbrir)
        //        {
        //            App.ExibirNotificacaoPush();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Workaround.ShowExceptionOnlyDevolpmentMode(nameof(OneSignalService), nameof(ExtrairETrabalharInformacoesRecebidas), ex);
        //    }
        //}
    }
}