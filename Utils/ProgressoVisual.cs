namespace MaCamp.Utils
{
    public class ProgressoVisual
    {
        public int Atual { get; private set; }
        public int Total { get; private set; }

        private ProgressBar ProgressBar { get; }

        public ProgressoVisual(ProgressBar progressBar)
        {
            ProgressBar = progressBar;

            progressBar.IsVisible = true;
            progressBar.Progress = 0;
        }

        public static async Task AumentarAtualAsync(ProgressoVisual? progressoVisual) => await AppConstants.CurrentPage.Dispatcher.DispatchAsync(async () =>
        {
            if (progressoVisual != null)
            {
                var valor = progressoVisual.Atual + 1;

                if (valor <= progressoVisual.Total)
                {
                    var proporcao = Convert.ToDouble(valor) / progressoVisual.Total;

                    progressoVisual.Atual = valor;

                    await progressoVisual.ProgressBar.ProgressTo(proporcao, 1, Easing.CubicIn);
                }
            }
        });

        public static void AumentarTotal(ProgressoVisual? progressoVisual, int valor)
        {
            if (progressoVisual != null && valor > 0)
            {
                progressoVisual.Total += valor;
            }
        }

        public static async Task CarregarDadosFakeAsync(ProgressBar progressBar, int quantidade)
        {
            var progressoVisual = new ProgressoVisual(progressBar);

            ProgressoVisual.AumentarTotal(progressoVisual, quantidade);

            await MaCamp.Utils.AppExtensions.ForEachAsync(Enumerable.Repeat(0, quantidade), async (x) =>
            {
                var delay = new Random().Next(750, 1500);

                await Task.Delay(delay);
                await AumentarAtualAsync(progressoVisual);
            });

            Console.WriteLine("ACABOUU");
        }
    }
}
