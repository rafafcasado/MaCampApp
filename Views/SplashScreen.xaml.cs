using MaCamp.Utils;

namespace MaCamp.Views
{
    public partial class SplashScreen : ContentPage
    {
        private Func<Task<Page>> Action { get; }

        public SplashScreen(Func<Task<Page>> task)
        {
            InitializeComponent();

            Action = task;

            Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object? sender, EventArgs e)
        {
            var duration = Convert.ToUInt32(500);

            try
            {
                await layout.ColorTo(Colors.White, AppColors.CorPrimaria, x => layout.BackgroundColor = x, duration);
                await image.FadeTo(1, duration, Easing.CubicInOut);

                var page = await Action.Invoke();

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
                await layout.ColorTo(Colors.Black, Colors.White, x => layout.BackgroundColor = x, duration);
                await image.FadeTo(0, duration, Easing.CubicInOut);
            }
        }
    }
}