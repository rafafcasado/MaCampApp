using Android.Gms.Ads;
using MaCamp.AppSettings;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;
using AdView = Android.Gms.Ads.AdView;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class AdmobBannerHandler : ViewHandler<AdMobBannerView, AdView>
    {
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

            // Limpar os recursos ao desconectar
            platformView.Destroy();
        }
    }
}