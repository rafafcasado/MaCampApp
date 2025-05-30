﻿using Android.Webkit;
using MaCamp.CustomControls;
using MaCamp.Platforms.Android.Handlers;
using MaCamp.Utils;
using WebView = Android.Webkit.WebView;

namespace MaCamp.Platforms.Android.Extenders
{
    public class ExtendedWebViewClient : WebViewClient
    {
        private CustomWebViewHandler Handler { get; }

        public ExtendedWebViewClient(CustomWebViewHandler handler)
        {
            Handler = handler;
        }

        public override async void OnPageFinished(WebView? view, string? url)
        {
            try
            {
                base.OnPageFinished(view, url);

                var previousHeight = 0;

                if (view != null && Handler.VirtualView is CustomWebView customWebView)
                {
                    var attempts = 30;

                    // Aguarda até que o conteúdo seja completamente renderizado
                    while ((view.ContentHeight == 0 || view.ContentHeight != previousHeight) && attempts-- > 0)
                    {
                        previousHeight = view.ContentHeight;

                        await Task.Delay(1000);
                    }

                    // Atualiza a altura do WebView dinamicamente
                    customWebView.HeightRequest = view.ContentHeight;
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(ExtendedWebViewClient), nameof(OnPageFinished), ex);
            }
        }

        public override bool ShouldOverrideUrlLoading(WebView? view, IWebResourceRequest? request)
        {
            var url = request?.Url?.ToString();

            if (url != null)
            {
                // Abre links externos no navegador padrão
                Workaround.TaskUI(async () => await Launcher.OpenAsync(url));
            }

            // retornar 'true' interrompe o carregamento no WebView
            return false;
        }
    }
}