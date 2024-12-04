using Aspbrasil.AppSettings;
using Google.MobileAds;
using Guia_de_Camping.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Aspbrasil.CustomControls.AdMobRectangleBannerView), typeof(AdMobRectangleBannerRenderer))]
namespace Guia_de_Camping.iOS.CustomRenderers
{
    public class AdMobRectangleBannerRenderer : ViewRenderer
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
                adView = new BannerView(AdSizeCons.MediumRectangle)
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

                e.NewElement.HeightRequest = 250;
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
    }
}