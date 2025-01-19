using MaCamp.AppSettings;

namespace MaCamp.Views
{
    public partial class SplashScreen : ContentPage
    {
        public SplashScreen(Type type)
        {
            InitializeComponent();

            AnimateSplashScreen(type);
        }

        private async void AnimateSplashScreen(Type type)
        {
            var duration = Convert.ToUInt32(System.Diagnostics.Debugger.IsAttached ? 50 : 500);

            try
            {
                await layout.ColorTo(Colors.White, Colors.Black, x => layout.BackgroundColor = x, duration);
                await image.FadeTo(1, duration, Easing.CubicInOut);

                var instance = Activator.CreateInstance(type);

                await Task.Delay(Convert.ToInt32(duration * 4));

                if (Application.Current != null && instance is Page page)
                {
                    Application.Current.MainPage = page;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await layout.ColorTo(Colors.Black, Colors.White, x => layout.BackgroundColor = x, duration);
                await image.FadeTo(0, duration, Easing.CubicInOut);
            }
        }
    }
}