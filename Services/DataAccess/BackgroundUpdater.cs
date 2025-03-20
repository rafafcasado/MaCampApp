using MaCamp.Dependencias;
using MaCamp.Models;
using MaCamp.Utils;
using MaCamp.ViewModels;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Services.DataAccess
{
    public static class BackgroundUpdater
    {
        private static CancellationTokenSource? CancellationTokenSource { get; set; }

        public static void CheckAndStart()
        {
            // Já está em execução
            if (CancellationTokenSource != null)
            {
                return;
            }

            var valor = DBContract.ObterValorChave(AppConstants.Chave_UltimaAtualizacao);

            if (DateTime.TryParse(valor, out var data) && data > DateTime.Now.Subtract(TimeSpan.FromDays(30)))
            {
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = CancellationTokenSource.Token;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var notificationService = await Workaround.GetServiceAsync<INotification>();
                    var notification = new NotificationData
                    {
                        Title = "Atualização do banco de dados",
                        Message = "Estamos atualizando as informações de Campings",
                        ProgressValue = -1
                    };
                    var notificationId = notificationService.Show(notification);

                    try
                    {
                        var vm = new ListagemInfinitaVM();

                        await Task.WhenAll(
                            Task.Run(async () =>
                            {
                                await CampingServices.CarregarCampings();

                                notification.ProgressValue += 50;
                            }),
                            Task.Run(async () =>
                            {
                                await vm.Carregar(string.Empty, -1, string.Empty, string.Empty, TipoListagem.Camping, false);

                                notification.ProgressValue += 50;
                            })
                        );

                        DBContract.InserirListaDeModelo(vm.Itens.ToList());
                        DBContract.InserirOuSubstituirModelo(new ChaveValor
                        {
                            Chave = AppConstants.Chave_UltimaAtualizacao,
                            Valor = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        });

                        notification.Message = "Concluída com sucesso";
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

                        Workaround.ShowExceptionOnlyDevolpmentMode(nameof(BackgroundUpdater), nameof(CheckAndStart), ex);
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
