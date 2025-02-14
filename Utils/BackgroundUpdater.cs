using MaCamp.Models;
using MaCamp.Models.Services;
using MaCamp.Services.DataAccess;
using MaCamp.ViewModels;

namespace MaCamp.Utils
{
    public class BackgroundUpdater
    {
        private CancellationTokenSource? CancellationTokenSource { get; set; }

        public void Start()
        {
            // Já está em execução
            if (CancellationTokenSource != null) return;

            var valor = DBContract.ObterValorChave(AppConstants.Chave_UltimaAtualizacao);

            if (valor is string chave && DateTime.TryParse(chave, out var data) && data > DateTime.Now.Subtract(TimeSpan.FromDays(30)))
            {
                return;
            }

            CancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = CancellationTokenSource.Token;

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var vm = new ListagemInfinitaVM();

                        await Task.WhenAll(
                            CampingServices.CarregarCampings(),
                            vm.Carregar("", -1, "", "", Enumeradores.TipoListagem.Camping, false)
                        );

                        DBContract.InserirListaDeModelo(vm.Itens.ToList());
                        DBContract.InserirOuSubstituirModelo(new ChaveValor
                        {
                            Chave = AppConstants.Chave_UltimaAtualizacao,
                            Valor = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                    catch (TaskCanceledException)
                    {
                        // Tarefa cancelada, interrompe o loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro durante a atualização: {ex.Message}");
                    }
                }
            }, cancellationToken);
        }

        public void Stop()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource = null;
        }
    }
}
