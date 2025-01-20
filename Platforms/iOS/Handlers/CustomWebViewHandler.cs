using MaCamp.Platforms.iOS.Extenders;
using Microsoft.Maui.Handlers;
using WebKit;

namespace MaCamp.Platforms.iOS.Handlers
{
    public partial class CustomWebViewHandler : WebViewHandler
    {
        protected override void ConnectHandler(WKWebView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.NavigationDelegate = new ExtendedWKNavigationDelegate(this);
        }

        protected override void DisconnectHandler(WKWebView platformView)
        {
            base.DisconnectHandler(platformView);

            platformView.NavigationDelegate = null!;
        }
    }
}