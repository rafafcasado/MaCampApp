using MaCamp.CustomControls;
using MaCamp.Utils;
using Microsoft.Maui.Handlers;
using MTAdmob.Google.MobileAds;
using UIKit;

namespace MaCamp.Platforms.iOS.Handlers
{
    public class AdmobRectangleBannerHandler : ViewHandler<AdmobRectangleBannerView, BannerView>
    {
        private static IPropertyMapper<AdmobRectangleBannerView, AdmobRectangleBannerHandler> Mapper => new PropertyMapper<AdmobRectangleBannerView, AdmobRectangleBannerHandler>(ViewMapper);

        public AdmobRectangleBannerHandler() : base(Mapper)
        {
        }

        protected override BannerView CreatePlatformView()
        {
            var adView = new BannerView
            {
                AdSize = AdSizeCons.GetCurrentOrientationAnchoredAdaptiveBannerAdSize(App.SCREEN_WIDTH),
                AdUnitID = AppConstants.AdmobIdBannerIOs,
                RootViewController = GetRootViewController()
            };

            adView.AdReceived += delegate
            {
                PlatformView.AddSubview(adView);
            };

            adView.LoadRequest(Request.GetDefaultRequest());

            VirtualView.HeightRequest = 250;

            return adView;
        }

        private UIViewController? GetRootViewController()
        {
            var listScenes = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().SelectMany(x => x.Windows).ToList();
            var viewController = listScenes.FirstOrDefault(x => x.IsKeyWindow)?.RootViewController;

            return viewController;
        }
    }
}