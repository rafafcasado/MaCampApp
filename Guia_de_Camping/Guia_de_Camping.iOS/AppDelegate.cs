using Aspbrasil;
using Foundation;
using Google.MobileAds;
using Keer.iOS.Dependencias;
using UIKit;
using Xamarin.Forms;

namespace Guia_de_Camping.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            App.ScreenPixelsSize = new Size(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            InicializarTrackingAnalytics();
            Xamarin.FormsGoogleMaps.Init("AIzaSyDMgtg637cz-GG0cimn_EXg95idLbaDwOY");
            LabelHtml.Forms.Plugin.iOS.HtmlLabelRenderer.Initialize();
            Rg.Plugins.Popup.Popup.Init();

            global::Xamarin.Forms.Forms.Init();

            GAService.GetGASInstance().Initialize_NativeGAS();


            MobileAds.Configure("ca-app-pub-8959365990645001~9967771040");

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
        private void InicializarTrackingAnalytics()
        {
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.TrackingId = Aspbrasil.AppSettings.GoogleAnalytics.TRACKING_ID;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppId = Aspbrasil.AppSettings.GoogleAnalytics.TRACKER_NAME;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppName = Aspbrasil.AppSettings.AppConstants.NOME_APP;

#if DEBUG
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.Debug = true;
#endif

            string version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
            int build = int.Parse(NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString());

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppVersion = $"{version}_{build}";
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.InitTracker();
        }
    }
}