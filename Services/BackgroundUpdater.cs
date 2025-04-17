using MaCamp.Dependencias;
using MaCamp.Utils;

namespace MaCamp.Services
{
    public static class BackgroundUpdater
    {
        private static CancellationTokenSource? CancellationTokenSource { get; set; }

        public static async Task StartAsync()
        {
            // Já está em execução
            if (CancellationTokenSource != null)
            {
                return;
            }

            var versaoAtual = 1.39;
            var chaveData = await DBContract.GetKeyValueAsync(AppConstants.Chave_UltimaAtualizacao);
            var chaveVersao = await DBContract.GetKeyValueAsync(AppConstants.Chave_Versao);

            if (DateTime.TryParse(chaveData, out var data) && data > DateTime.Now.Subtract(TimeSpan.FromDays(30)) && double.TryParse(chaveVersao, out var versao) && versao >= versaoAtual)
            {
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = CancellationTokenSource.Token;

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
                    var openNotification = await DBContract.GetKeyValueAsync(AppConstants.Chave_NotificacaoAberta);

                    if (openNotification != null)
                    {
                        notificationService.Complete(Convert.ToInt32(openNotification));
                    }

                    await DBContract.UpdateKeyValueAsync(AppConstants.Chave_NotificacaoAberta, Convert.ToString(notificationId));

                    try
                    {
                        await CampingServices.BaixarCampingsAsync(true);

                        notification.Message = "Estamos salvando informações de Campings";

                        notificationService.Update(notificationId, notification);

                        await DBContract.UpdateKeyValueAsync(AppConstants.Chave_UltimaAtualizacao, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        await DBContract.UpdateKeyValueAsync(AppConstants.Chave_Versao, versaoAtual.ToString());
                        await DBContract.UpdateKeyValueAsync(AppConstants.Chave_NotificacaoAberta, null);

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
                    CancellationTokenSource.Cancel();
                }
            });
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
