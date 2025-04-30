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
                switch (webView.Source)
                {
                    case HtmlWebViewSource htmlSource:
                        handler.NativeWebView.LoadDataWithBaseURL(null, htmlSource.Html, "text/html", "utf-8", null);
                        break;
                    case UrlWebViewSource urlSource:
                        handler.NativeWebView.LoadUrl(urlSource.Url);
                        break;
                }
            }
        }

        protected override void ConnectHandler(WebView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.Settings.DomStorageEnabled = true;
            platformView.Settings.JavaScriptEnabled = true;
            platformView.Settings.SetGeolocationEnabled(true);
            platformView.Settings.JavaScriptCanOpenWindowsAutomatically = true;

            platformView.SetWebViewClient(new ExtendedWebViewClient(this));
            platformView.SetWebChromeClient(new ExtendedWebChromeClient());
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