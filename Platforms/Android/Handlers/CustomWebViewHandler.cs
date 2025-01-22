using MaCamp.CustomControls;
using MaCamp.Platforms.Android.Extenders;
using Microsoft.Maui.Handlers;
using WebView = Android.Webkit.WebView;

namespace MaCamp.Platforms.Android.Handlers
{
    public class CustomWebViewHandler : WebViewHandler
    {
        public new static IPropertyMapper<IWebView, CustomWebViewHandler> Mapper = new PropertyMapper<IWebView, CustomWebViewHandler>(WebViewHandler.Mapper)
        {
            [nameof(Microsoft.Maui.Controls.WebView.Source)] = MapSource
        };

        private static CustomWebView? CustomWebView { get; set; }
        private WebView? NativeWebView { get; set; }

        public CustomWebViewHandler() : base(Mapper)
        {
        }

        protected override WebView CreatePlatformView()
        {
            NativeWebView = new WebView(Context);

            NativeWebView.Settings.JavaScriptEnabled = true;
            NativeWebView.SetWebViewClient(new ExtendedWebViewClient(this));
            NativeWebView.SetWebChromeClient(new ExtendedWebChromeClient());

            return NativeWebView;
        }

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);

            if (view is CustomWebView customWebView)
            {
                CustomWebView = customWebView;
            }
        }

        private static void MapSource(CustomWebViewHandler handler, IWebView webView)
        {
            if (handler.NativeWebView != null)
            {
                if (webView.Source is HtmlWebViewSource htmlSource)
                {
                    handler.NativeWebView.LoadDataWithBaseURL(null, htmlSource.Html, "text/html", "utf-8", null);
                }
                else if (webView.Source is UrlWebViewSource urlSource)
                {
                    handler.NativeWebView.LoadUrl(urlSource.Url);
                }
            }
        }

        protected override void DisconnectHandler(WebView platformView)
        {
            base.DisconnectHandler(platformView);

            if (NativeWebView != null)
            {
                platformView.StopLoading();
                platformView.LoadUrl("about:blank");
            }
        }
    }
}