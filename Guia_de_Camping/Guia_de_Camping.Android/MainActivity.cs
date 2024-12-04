using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Aspbrasil;
using Aspbrasil.Models.DataAccess;
using Plugin.Permissions;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;
using Android.Preferences;
using Android.Gms;


namespace Guia_de_Camping.Droid
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]


    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            App.ScreenPixelsSize = new Xamarin.Forms.Size((int)(Resources.DisplayMetrics.WidthPixels), (int)(Resources.DisplayMetrics.HeightPixels));

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);

            Forms.SetFlags("FastRenderers_Experimental");
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            Xamarin.FormsGoogleMaps.Init(this, bundle);
            LabelHtml.Forms.Plugin.Droid.HtmlLabelRenderer.Initialize();
            Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            //MobileAds.Initialize(this);

            InicializarTrackingAnalytics();
            LoadApplication(new App());
        }

        private void InicializarTrackingAnalytics()
        {
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.TrackingId = Aspbrasil.AppSettings.GoogleAnalytics.TRACKING_ID;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppId = Aspbrasil.AppSettings.GoogleAnalytics.TRACKER_NAME;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppName = Aspbrasil.AppSettings.AppConstants.NOME_APP;

            Context context = this.ApplicationContext;
            string version = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppVersion = version;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.InitTracker();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



    }




}



