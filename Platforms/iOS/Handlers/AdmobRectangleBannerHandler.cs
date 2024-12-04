using Google.MobileAds;
using MaCamp.AppSettings;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;
using UIKit;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class AdmobRectangleBannerHandler : ViewHandler<AdmobRectangleBannerView, BannerView>
    {
        protected override BannerView CreatePlatformView()
        {
            var adView = new BannerView
            {
                AdSize = AdSizeCons.MediumRectangle,
                AdUnitId = AppConstants.AdmobIdBannerIOs,
                RootViewController = GetRootViewController()
            };

            adView.AdReceived += delegate
            {
                PlatformView.AddSubview(adView);
            };

            adView.LoadRequest(Request.GetDefaultRequest());

            // Ajusta a altura da View
            VirtualView.HeightRequest = 250;

            return adView;
        }

        private UIViewController? GetRootViewController()
        {
            var listScenes = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>().SelectMany(scene => scene.Windows).ToList();
            var viewController = listScenes.FirstOrDefault(window => window.IsKeyWindow)?.RootViewController;

            return viewController;
        }
    }
}