using Windows.ApplicationModel;

namespace Guia_de_Camping.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            InicializarTrackingAnalytics();

            LoadApplication(new Aspbrasil.App());
        }
        private void InicializarTrackingAnalytics()
        {
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.TrackingId = Aspbrasil.AppSettings.GoogleAnalytics.TRACKING_ID;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppId = Aspbrasil.AppSettings.GoogleAnalytics.TRACKER_NAME;
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppName = Aspbrasil.AppSettings.AppConstants.NOME_APP;

#if DEBUG
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.Debug = true;
#endif

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            Plugin.GoogleAnalytics.GoogleAnalytics.Current.Config.AppVersion = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            Plugin.GoogleAnalytics.GoogleAnalytics.Current.InitTracker();
        }
    }
}
