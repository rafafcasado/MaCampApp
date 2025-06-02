using Foundation;
using Google.Maps;
using MaCamp.Utils;
using UIKit;

namespace MaCamp.Platforms.iOS
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            MapServices.ProvideApiKey(AppConstants.ApiKey_Mapa);

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}