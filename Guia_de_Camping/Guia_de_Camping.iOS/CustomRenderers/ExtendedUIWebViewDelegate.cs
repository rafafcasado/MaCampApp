using Aspbrasil.CustomControls;
using Aspbrasil.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedWebView), typeof(ExtendedWebViewRenderer))]
namespace Aspbrasil.iOS.CustomRenderers
{
    public class ExtendedWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            Delegate = new ExtendedUIWebViewDelegate(this);
        }
    }

    public class ExtendedUIWebViewDelegate : UIWebViewDelegate
    {
        ExtendedWebViewRenderer webViewRenderer;

        public ExtendedUIWebViewDelegate(ExtendedWebViewRenderer _webViewRenderer = null)
        {
            webViewRenderer = _webViewRenderer ?? new ExtendedWebViewRenderer();

        }

        public override void LoadStarted(UIWebView webView)
        {
            MessagingCenter.Send(App.Current, "ATUALIZAR_PROGRESSO_WEBVIEW", 20);
        }

        public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            if (navigationType == UIWebViewNavigationType.LinkClicked)
            {
                string url = request.Url.ToString();
                //if (url.StartsWith(AppConstants.SCHEMA))
                //{
                //    System.Threading.Tasks.Task.Run(async () => await Models.Schema.ResolverFuncao(url));
                //}
                //else
                //{
                    Device.OpenUri(new System.Uri(url));
                //}
                return false;
            }

            return true;
        }

        public override async void LoadingFinished(UIWebView webView)
        {
            try
            {
                var extendedWebView = webViewRenderer.Element as ExtendedWebView;
                if (extendedWebView != null)
                {
                    MessagingCenter.Send(App.Current, "ATUALIZAR_PROGRESSO_WEBVIEW", 100);

                    int i = 30;
                    double alturaAntiga = 0;
                    while (((double)webView.ScrollView.ContentSize.Height == 0 || (double)webView.ScrollView.ContentSize.Height != alturaAntiga) && i-- > 0) // aguarda o conteudo ser renderizado
                    {
                        alturaAntiga = (double)webView.ScrollView.ContentSize.Height;
                        await System.Threading.Tasks.Task.Delay(1000);
                    }
                    extendedWebView.HeightRequest = (double)webView.ScrollView.ContentSize.Height;
                }
            }
            catch
            {
                //Try para garantir que quando o usuário passar rapidamente pelos itens, e o WebView sofrer dispose, não tentará exibir o conteúdo;
            }
        }
    }
}