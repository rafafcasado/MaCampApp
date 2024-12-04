using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Aspbrasil.AppSettings;
using Aspbrasil.CustomControls;
using Guia_de_Camping.Droid.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobBannerView), typeof(AdMobBannerRenderer))]
namespace Guia_de_Camping.Droid.CustomControls
{
    public class AdMobBannerRenderer : ViewRenderer
    {
        public AdMobBannerRenderer(Context context) : base(context)
        {

        }

        private int GetSmartBannerDpHeight()
        {
            var dpHeight = Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density;

            if (dpHeight <= 400) return 32;
            if (dpHeight <= 720) return 50;
            return 90;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var ad = new AdView(Context)
                {
                    AdSize = AdSize.SmartBanner,
                    AdUnitId = AppConstants.ADMOB_ID_BANNER_Android
                };

                var builder = new AdRequest.Builder();
                var requestbuilder = builder;

                ad.LoadAd(requestbuilder.Build());
                e.NewElement.HeightRequest = GetSmartBannerDpHeight();

                SetNativeControl(ad);
            }
        }
    }
}