using Android.Gms.Ads;
using MaCamp.CustomControls;
using MaCamp.Utils;
using Microsoft.Maui.Handlers;
using AdView = Android.Gms.Ads.AdView;

namespace MaCamp.Platforms.Android.Handlers
{
    public class AdmobBannerHandler : ViewHandler<AdMobBannerView, AdView>
    {
        private static IPropertyMapper<AdMobBannerView, AdmobBannerHandler> Mapper => new PropertyMapper<AdMobBannerView, AdmobBannerHandler>(ViewMapper);

        public AdmobBannerHandler() : base(Mapper)
        {
        }

        private int GetSmartBannerDpHeight()
        {
            if (Context.Resources?.DisplayMetrics != null)
            {
                var dpHeight = Context.Resources.DisplayMetrics.HeightPixels / Context.Resources.DisplayMetrics.Density;

                if (dpHeight <= 400)
                {
                    return 32;
                }

                if (dpHeight <= 720)
                {
                    return 50;
                }
            }

            return 90;
        }

        protected override AdView CreatePlatformView()
        {
            var adView = new AdView(Context)
            {
                AdSize = AdSize.Banner,
                AdUnitId = AppConstants.AdmobIdBannerAndroid
            };
            var adRequest = new AdRequest.Builder().Build();

            adView.LoadAd(adRequest);

            return adView;
        }

        protected override void ConnectHandler(AdView platformView)
        {
            base.ConnectHandler(platformView);

            if (platformView.LayoutParameters != null)
            {
                platformView.LayoutParameters.Height = GetSmartBannerDpHeight();
            }
        }

        protected override void DisconnectHandler(AdView platformView)
        {
            base.DisconnectHandler(platformView);

            platformView.Destroy();
        }
    }
}