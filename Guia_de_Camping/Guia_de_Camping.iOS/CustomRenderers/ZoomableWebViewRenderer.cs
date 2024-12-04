using Aspbrasil.CustomControls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebView), typeof(ZoomableWebViewRenderer))]
namespace Aspbrasil.CustomControls
{
    public class ZoomableWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            var view = (UIWebView)NativeView;
            view.ScrollView.ScrollEnabled = true;
            view.ScalesPageToFit = true;
        }
    }
}