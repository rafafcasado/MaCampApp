using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MaCamp.AppSettings;
using MaCamp.Platforms.Android.Utils;
using Color = Android.Graphics.Color;

namespace MaCamp.Platforms.Android
{
    [Activity(Theme = "@style/Theme.Main", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = SoftInput.AdjustResize, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private bool IsSplashScreenActive { get; set; }

        public MainActivity()
        {
            IsSplashScreenActive = true;
        }

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Window != null)
            {
                Window.DecorView.SetOnApplyWindowInsetsListener(new InsetsListener(() => !IsSplashScreenActive));
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (App.Current != null)
            {
                App.Current.PageAppearing += (s, e) =>
                {
                    if (e is not Views.SplashScreen)
                    {
                        IsSplashScreenActive = false;

                        RunOnUiThread(() =>
                        {
                            if (Window != null)
                            {
                                Window.SetDecorFitsSystemWindows(false);
                                Window.DecorView.SetBackgroundColor(Color.ParseColor(AppColors.CorPrimaria.ToHex()));
                            }
                        });
                    }
                };
            }
        }
    }
}