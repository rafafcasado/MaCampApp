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

[assembly: ExportRenderer(typeof(AdMobRectangleBannerView), typeof(AdMobRectangleBannerRenderer))]
namespace Guia_de_Camping.Droid.CustomControls
{
    public class AdMobRectangleBannerRenderer : ViewRenderer
    {
        public AdMobRectangleBannerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var ad = new AdView(Context)
                {
                    AdSize = AdSize.MediumRectangle,
                    AdUnitId = AppConstants.ADMOB_ID_BANNER_Android
                };

                var builder = new AdRequest.Builder();
                var requestbuilder = builder;

                ad.LoadAd(requestbuilder.Build());
                e.NewElement.HeightRequest = 250;

                SetNativeControl(ad);
            }
        }
    }
}