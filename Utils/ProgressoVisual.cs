namespace MaCamp.Utils
{
    public class ProgressoVisual
    {
        private int Atual { get; set; }
        private int Total { get; set; }

        private ProgressBar ProgressBar { get; }

        public ProgressoVisual(ProgressBar progressBar)
        {
            ProgressBar = progressBar;

            ProgressBar.IsVisible = true;
            ProgressBar.Progress = 0;
        }

        public static async void AumentarAtual(ProgressoVisual? progressoVisual, int? amout = null)
        {
            if (progressoVisual != null)
            {
                var valor = progressoVisual.Atual + (amout ?? 1);

                if (valor <= progressoVisual.Total)
                {
                    var proporcao = Convert.ToDouble(valor) / progressoVisual.Total;

                    progressoVisual.Atual = valor;

                    await Workaround.TaskUIAsync(async () =>
                    {
                        await progressoVisual.ProgressBar.ProgressTo(proporcao, 500, Easing.CubicIn);
                    });
                }
            }
        }

        public static void AumentarTotal(ProgressoVisual? progressoVisual, int valor)
        {
            if (progressoVisual != null && valor > 0)
            {
                progressoVisual.Total += valor;
            }
        }
    }
}
