using MaCamp.Utils;

namespace MaCamp.Views
{
    public partial class SplashScreen : ContentPage
    {
        private Func<Task<Page>> PredicateTask { get; }

        public SplashScreen(Func<Task<Page>> predicateTask)
        {
            InitializeComponent();

            PredicateTask = predicateTask;

            Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object? sender, EventArgs e)
        {
            var duration = Convert.ToUInt32(500);

            try
            {
                await Task.WhenAll(
                    layout.ColorTo(Colors.White, AppColors.CorPrimaria, x => layout.BackgroundColor = x, duration),
                    image.FadeTo(1, duration, Easing.CubicInOut)
                );

                var page = await PredicateTask();

                await Task.Delay(Convert.ToInt32(duration * 4));

                if (Application.Current != null)
                {
                    Application.Current.MainPage = page;
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(SplashScreen), nameof(SplashScreen_Loaded), ex);
            }
            finally
            {
                await Task.WhenAll(
                    layout.ColorTo(Colors.Black, Colors.White, x => layout.BackgroundColor = x, duration),
                    image.FadeTo(0, duration, Easing.CubicInOut)
                );
            }
        }
    }
}