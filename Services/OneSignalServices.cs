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

        //private async void HandlerNotificacaoRecebida(OSNotification notification)
        //{
        //    await ExtrairETrabalharInformacoesRecebidasAsync(notification.payload, false);
        //}

        //private void HandlerNotificacaoAberta(OSNotificationOpenedResult result)
        //{
        //    await ExtrairETrabalharInformacoesRecebidasAsync(result.notification.payload);
        //}

        //private async Task ExtrairETrabalharInformacoesRecebidasAsync(OSNotificationPayload payload, bool tentarAbrir = true)
        //{
        //    try
        //    {
        //        Dictionary<string, object> additionalData = payload.additionalData;

        //        await DBContract.UpdateKeyValue(AppConstants.CHAVE_TITULO_NOTIFICACAO, payload.title);
        //        await DBContract.UpdateKeyValue(AppConstants.CHAVE_MENSAGEM_NOTIFICACAO, payload.body);

        //        if (additionalData != null && additionalData.Count > 0)
        //        {
        //            if (additionalData.ContainsKey("id"))
        //            {
        //                var id = additionalData["id"].ToString();
        //                await DBContract.UpdateKeyValue(AppConstants.CHAVE_ID_ITEM_NOTIFICACAO, id);
        //            }
        //        }

        //        if (payload.launchURL != null)
        //        {
        //            await DBContract.UpdateKeyValue(AppConstants.CHAVE_URL_NOTIFICACAO, payload.launchURL);
        //        }

        //        if (tentarAbrir)
        //        {
        //            await App.ExibirNotificacaoPushAsync();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Workaround.ShowExceptionOnlyDevolpmentMode(nameof(OneSignalService), nameof(ExtrairETrabalharInformacoesRecebidasAsync), ex);
        //    }
        //}
    }
}