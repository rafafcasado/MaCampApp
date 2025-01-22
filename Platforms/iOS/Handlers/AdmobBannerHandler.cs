using Google.MobileAds;
using MaCamp.CustomControls;
using MaCamp.Utils;
using Microsoft.Maui.Handlers;
using UIKit;

namespace MaCamp.Platforms.iOS.Handlers
{
    public class AdmobBannerHandler : ViewHandler<AdMobBannerView, BannerView>
    {
        private BannerView? AdView { get; set; }

        private static IPropertyMapper<AdMobBannerView, AdmobBannerHandler> Mapper => new PropertyMapper<AdMobBannerView, AdmobBannerHandler>(ViewMapper);

        public AdmobBannerHandler() : base(Mapper)
        {
        }

        protected override BannerView CreatePlatformView()
        {
            AdView = new BannerView
            {
                AdSize = AdSizeCons.Banner,
                AdUnitId = AppConstants.AdmobIdBannerIOs,
                RootViewController = GetRootViewController()
            };

            AdView.AdReceived += OnAdReceived;

            var request = Request.GetDefaultRequest();

            AdView.LoadRequest(request);

            return AdView;
        }

        private UIViewController GetRootViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;

            if (window?.RootViewController != null)
            {
                return window.RootViewController;
            }

            throw new InvalidOperationException("Não foi possível localizar o RootViewController.");
        }

        protected override void DisconnectHandler(BannerView platformView)
        {
            if (AdView != null)
            {
                AdView.AdReceived -= OnAdReceived;
                AdView.RemoveFromSuperview();
                AdView.Dispose();
                AdView = null;
            }

            base.DisconnectHandler(platformView);
        }

        private void OnAdReceived(object? sender, EventArgs e)
        {
            if (AdView != null)
            {
                PlatformView.AddSubview(AdView);
            }
        }
    }
}