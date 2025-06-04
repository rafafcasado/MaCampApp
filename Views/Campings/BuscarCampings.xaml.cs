using MaCamp.CustomControls;
using MaCamp.Services;
using MaCamp.Utils;

namespace MaCamp.Views.Campings
{
    public partial class BuscarCampings : SmartContentPage
    {
        public BuscarCampings()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            FirstAppeared += BuscarCampings_FirstAppeared;

            //Plugin.GoogleAnalytics.GoogleAnalytics.Current.Tracker.SendView("Buscar Campings Atualizados");
        }

        private async void BuscarCampings_FirstAppeared(object? sender, EventArgs e)
        {
            var progressoVisual = new ProgressoVisual(progressBar);

            DeviceDisplay.KeepScreenOn = true;

            await Task.WhenAll(
                CidadesServices.AtualizarListaCidadesAsync(progressoVisual),
                CampingServices.BaixarCampingsAsync(true, progressoVisual)
            );
            await DBContract.UpdateKeyValue(AppConstants.Chave_UltimaAtualizacao, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            await Navigation.PopAsync();
        }
    }
}