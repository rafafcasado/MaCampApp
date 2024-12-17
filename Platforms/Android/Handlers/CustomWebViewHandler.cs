using MaCamp.CustomControls;
using MaCamp.Platforms.Android.Extenders;
using Microsoft.Maui.Handlers;
using WebView = Android.Webkit.WebView;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class CustomWebViewHandler : WebViewHandler
    {
        private static CustomWebView? CustomWebView { get; set; }
        private WebView? NativeWebView { get; set; }

        protected override WebView CreatePlatformView()
        {
            NativeWebView = new WebView(Context);

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