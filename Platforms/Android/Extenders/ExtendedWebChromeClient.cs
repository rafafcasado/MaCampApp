using Android.Webkit;
using MaCamp.Utils;
using WebView = Android.Webkit.WebView;

namespace MaCamp.Platforms.Android.Extenders
{
    public class ExtendedWebChromeClient : WebChromeClient
    {
        public override void OnProgressChanged(WebView? view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            // Envia mensagem para atualizar o progresso da página
            MessagingCenter.Send(AppConstants.CurrentPage, "ATUALIZAR_PROGRESSO_WEBVIEW", newProgress);
        }
    }
}