using Android.Webkit;
using CommunityToolkit.Mvvm.Messaging;
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
            WeakReferenceMessenger.Default.Send<object, string>(newProgress, AppConstants.WeakReferenceMessenger_AtualizarProgressoWebView);
        }
    }
}