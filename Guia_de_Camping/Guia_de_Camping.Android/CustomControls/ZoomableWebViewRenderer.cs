using Android.Content;
using Aspbrasil.CustomControls;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(WebView), typeof(ZoomableWebViewRenderer))]
namespace Aspbrasil.CustomControls
{
    public class ZoomableWebViewRenderer : WebViewRenderer
    {
        public ZoomableWebViewRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Control != null)
            {
                Control.Settings.BuiltInZoomControls = true;
                Control.Settings.DisplayZoomControls = false;
            }
            base.OnElementPropertyChanged(sender, e);
        }

    }
}