using Android.Gms.Ads;
using MaCamp.AppSettings;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class AdmobRectangleBannerHandler : ViewHandler<AdmobRectangleBannerView, AdView>
    {
        protected override AdView CreatePlatformView()
        {
            var adView = new AdView(Context)
            {
                AdSize = AdSize.MediumRectangle,
                AdUnitId = AppConstants.AdmobIdBannerAndroid
            };

            var builder = new AdRequest.Builder();
            adView.LoadAd(builder.Build());
            VirtualView.HeightRequest = 250;

            return adView;
        }
    }
}