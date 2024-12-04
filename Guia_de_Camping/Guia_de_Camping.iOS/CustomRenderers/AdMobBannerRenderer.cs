using Aspbrasil.AppSettings;
using Google.MobileAds;
using Guia_de_Camping.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Aspbrasil.CustomControls.AdMobBannerView), typeof(AdMobBannerRenderer))]
namespace Guia_de_Camping.iOS.CustomRenderers
{
    public class AdMobBannerRenderer : ViewRenderer
    {
        BannerView adView;
        bool viewOnScreen;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (e.OldElement == null)
            {
                adView = new BannerView(AdSizeCons.SmartBannerPortrait)
                {
                    AdUnitID = AppConstants.ADMOB_ID_BANNER_iOS,
                    RootViewController = GetRootViewController()
                };

                adView.AdReceived += (sender, args) =>
                {
                    if (!viewOnScreen) this.AddSubview(adView);
                    viewOnScreen = true;
                };
                var request = Request.GetDefaultRequest();
                e.NewElement.HeightRequest = GetSmartBannerDpHeight();
                adView.LoadRequest(request);
                base.SetNativeControl(adView);
            }
        }

        private UIViewController GetRootViewController()
        {
            foreach (UIWindow window in UIApplication.SharedApplication.Windows)
            {
                if (window.RootViewController != null)
                {
                    return window.RootViewController;
                }
            }

            return null;
        }

        private int GetSmartBannerDpHeight()
        {
            var dpHeight = (double)UIScreen.MainScreen.Bounds.Height;

            if (dpHeight <= 400) return 32;
            if (dpHeight <= 720) return 50;
            return 90;
        }
    }
}