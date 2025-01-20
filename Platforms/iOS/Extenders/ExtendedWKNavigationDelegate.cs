using MaCamp.CustomControls;
using MaCamp.Platforms.iOS.Handlers;
using MaCamp.Utils;
using WebKit;

namespace MaCamp.Platforms.iOS.Extenders
{
    public class ExtendedWKNavigationDelegate : WKNavigationDelegate
    {
        private CustomWebViewHandler Handler { get; set; }

        public ExtendedWKNavigationDelegate(CustomWebViewHandler handler)
        {
            Handler = handler;
        }

        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            // Envia mensagem para atualizar o progresso
            MessagingCenter.Send(AppConstants.CurrentPage, "ATUALIZAR_PROGRESSO_WEBVIEW", 20);
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            try
            {
                if (Handler.VirtualView is CustomWebView extendedWebView)
                {
                    // Atualiza o progresso
                    MessagingCenter.Send(AppConstants.CurrentPage, "ATUALIZAR_PROGRESSO_WEBVIEW", 100);

                    // Ajusta a altura do WebView dinamicamente após o carregamento do conteúdo
                    var attempts = 30;
                    var previousHeight = 0;

                    while ((webView.ScrollView.ContentSize.Height == 0 || webView.ScrollView.ContentSize.Height != previousHeight) && attempts-- > 0)
                    {
                        previousHeight = (int)webView.ScrollView.ContentSize.Height;
                        extendedWebView.HeightRequest = webView.ScrollView.ContentSize.Height;

                        // Aguarda para garantir que o conteúdo seja renderizado.
                        Task.Delay(1000).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                // Proteção para evitar exceções durante o descarte do controle
                Console.WriteLine(ex.Message);
            }
        }

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var url = navigationAction.Request.Url.ToString();

            if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
            {
                // Abre links externos no navegador padrão
                Task.Run(async () => await Launcher.OpenAsync(url));

                // Cancela a navegação no WebView para links externos
                decisionHandler(WKNavigationActionPolicy.Cancel);
            }
            else
            {
                // Permite a navegação dentro do WebView
                decisionHandler(WKNavigationActionPolicy.Allow);
            }
        }
    }
}