using System.Globalization;
using MaCamp.Dependencias;
using MaCamp.Utils;

namespace MaCamp.Services
{
    public static class BackgroundUpdater
    {
        private static CancellationTokenSource? CancellationTokenSource { get; set; }

        public static async Task StartAsync(bool force = false)
        {
            // Já está em execução
            if (CancellationTokenSource != null)
            {
                return;
            }

            var versaoAtual = 1.39;

            if (!force)
            {
                var chaveData = DBContract.GetKeyValue(AppConstants.Chave_UltimaAtualizacao);
                var chaveVersao = DBContract.GetKeyValue(AppConstants.Chave_Versao);

                if (DateTime.TryParse(chaveData, out var data) && data > DateTime.Now.Subtract(AppConstants.Tempo_RotinaAtualizacao) && double.TryParse(chaveVersao, out var versao) && versao >= versaoAtual)
                {
                    return;
                }
            }

            CancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = CancellationTokenSource.Token;
            var permission = await Workaround.CheckPermissionAsync<Permissions.PostNotifications>("Notificação", "Forneça a permissão para exibir os status das atualizações de dados");

            if (permission)
            {

                await Workaround.TaskWorkAsync(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var notificationService = await Workaround.GetServiceAsync<INotification>();
                        var notification = new NotificationData
                        {
                            Title = "Atualização do banco de dados",
                            Message = "Estamos buscando informações de Campings",
                            ProgressValue = -1
                        };
                        var notificationId = notificationService.Show(notification);
                        var openNotification = DBContract.GetKeyValue(AppConstants.Chave_NotificacaoAberta);

                        if (openNotification != null)
                        {
                            notificationService.Complete(Convert.ToInt32(openNotification));
                        }

                        DBContract.UpdateKeyValue(AppConstants.Chave_NotificacaoAberta, Convert.ToString(notificationId));

                        try
                        {
                            await CampingServices.BaixarCampingsAsync(true);

                            notification.Message = "Estamos salvando informações de Campings";

                            notificationService.Update(notificationId, notification);

                            DBContract.UpdateKeyValue(AppConstants.Chave_UltimaAtualizacao, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            DBContract.UpdateKeyValue(AppConstants.Chave_Versao, versaoAtual.ToString(CultureInfo.InvariantCulture));
                            DBContract.UpdateKeyValue(AppConstants.Chave_NotificacaoAberta, null);

                            notificationService.Complete(notificationId);
                        }
                        catch (TaskCanceledException)
                        {
                            notification.Message = "A atualização foi cancelada";
                            // Tarefa cancelada, interrompe o loop
                            break;
                        }
                        catch (Exception ex)
                        {
                            notification.Title = "Ocorreu um erro, tentaremos novamente mais tarde";

                            Workaround.ShowExceptionOnlyDevolpmentMode(nameof(BackgroundUpdater), nameof(StartAsync), ex);
                        }

                        notification.ProgressValue = null;

                        notificationService.Update(notificationId, notification);

                        if (CancellationTokenSource != null)
                        {
                            await CancellationTokenSource.CancelAsync();
                        }

                        CancellationTokenSource = null;
                    }
                }, cancellationToken);
            }
        }

        public static void Stop()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource = null;
            }
        }
    }
}
