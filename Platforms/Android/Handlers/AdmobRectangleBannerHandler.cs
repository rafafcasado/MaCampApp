using Android.Gms.Ads;
using MaCamp.CustomControls;
using MaCamp.Utils;
using Microsoft.Maui.Handlers;

namespace MaCamp.Platforms.Android.Handlers
{
    public class AdmobRectangleBannerHandler : ViewHandler<AdmobRectangleBannerView, AdView>
    {
        private static IPropertyMapper<AdmobRectangleBannerView, AdmobRectangleBannerHandler> Mapper => new PropertyMapper<AdmobRectangleBannerView, AdmobRectangleBannerHandler>(ViewMapper);

        public AdmobRectangleBannerHandler() : base(Mapper)
        {
        }

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