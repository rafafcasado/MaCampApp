using MaCamp.Utils;

namespace MaCamp.Views
{
    public partial class SplashScreen : ContentPage
    {
        public SplashScreen(Func<Task<Page>> task)
        {
            InitializeComponent();

            Animated(task);
        }

        private async void Animated(Func<Task<Page>> task)
        {
            var duration = Convert.ToUInt32(500);

            try
            {
                await layout.ColorTo(Colors.White, AppColors.CorPrimaria, x => layout.BackgroundColor = x, duration);
                await image.FadeTo(1, duration, Easing.CubicInOut);

                var page = await task.Invoke();

                await Task.Delay(Convert.ToInt32(duration * 4));

                if (Application.Current != null)
                {
                    Application.Current.MainPage = page;
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(SplashScreen), nameof(Animated), ex);
            }
            finally
            {
                await layout.ColorTo(Colors.Black, Colors.White, x => layout.BackgroundColor = x, duration);
                await image.FadeTo(0, duration, Easing.CubicInOut);
            }
        }
    }
}