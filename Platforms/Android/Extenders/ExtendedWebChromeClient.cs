using Android.Webkit;
using CommunityToolkit.Mvvm.Messaging;
using MaCamp.Utils;
using WebView = Android.Webkit.WebView;

namespace MaCamp.Platforms.Android.Extenders
{
    public class ExtendedWebChromeClient : WebChromeClient
    {
        public override void OnPermissionRequest(PermissionRequest? request)
        {
            if (request != null)
            {
                request.Grant(request.GetResources());
            }
        }

        public override void OnGeolocationPermissionsShowPrompt(string? origin, GeolocationPermissions.ICallback? callback)
        {
            if (callback != null)
            {
                callback.Invoke(origin, true, false);
            }
        }

        public override void OnProgressChanged(WebView? view, int newProgress)
        {
            base.OnProgressChanged(view, newProgress);

            // Envia mensagem para atualizar o progresso da página
            WeakReferenceMessenger.Default.Send<object, string>(newProgress, AppConstants.WeakReferenceMessenger_AtualizarProgressoWebView);
        }
    }
}