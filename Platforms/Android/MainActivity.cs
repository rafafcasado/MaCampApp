using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using MaCamp.Platforms.Android.Utils;
using MaCamp.Utils;
using Color = Android.Graphics.Color;

namespace MaCamp.Platforms.Android
{
    [Activity(Theme = "@style/Theme.SplashScreen", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = SoftInput.AdjustResize, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Window != null)
            {
                var color = Color.ParseColor(AppColors.CorPrimaria.ToHex());

                Window.DecorView.SetOnApplyWindowInsetsListener(new InsetsListener
                {
                    MainColor = color
                });
            }
        }
    }

}