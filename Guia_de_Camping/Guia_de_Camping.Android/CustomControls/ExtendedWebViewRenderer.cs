using Android.Content;
using Aspbrasil.CustomControls;
using Aspbrasil.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(ExtendedWebView), typeof(ExtendedWebViewRenderer))]
namespace Aspbrasil.Droid.CustomRenderers
{
    public class ExtendedWebViewRenderer : WebViewRenderer
    {
        public ExtendedWebViewRenderer(Context context) : base(context)
        {
            AutoPackage = false;
        }

        //static bool dettached = false;
        static ExtendedWebView ExtendedWebView = null;
        Android.Webkit.WebView NativeWebView = null;

        class ExtendedWebViewClient : Android.Webkit.WebViewClient
        {
            public override async void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                //if (dettached)
                //{
                //    dettached = false;
                //    return;
                //}
                try
                {
                    if (ExtendedWebView != null)
                    {
                        int i = 30;
                        int alturaAntiga = 0;
                        while ((view.ContentHeight == 0 || view.ContentHeight != alturaAntiga) && i-- > 0) // aguarda o conteudo ser renderizado
                        {
                            alturaAntiga = view.ContentHeight;
                            await System.Threading.Tasks.Task.Delay(1000);
                        }
                        ExtendedWebView.HeightRequest = view.ContentHeight;

                        base.OnPageFinished(view, url);
                    }
                }
                catch
                {
                    //Try para garantir que quando o usuário passar rapidamente pelos itens, e o WebView sofrer dispose, não tentará exibir o conteúdo;
                }
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, Android.Webkit.IWebResourceRequest request)
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
                return true;
            }
        }

        class ExtendedWebViewChrome : Android.Webkit.WebChromeClient
        {
            public override void OnProgressChanged(Android.Webkit.WebView view, int newProgress)
            {
                MessagingCenter.Send<Xamarin.Forms.Application, int>(App.Current, "ATUALIZAR_PROGRESSO_WEBVIEW", newProgress);
                base.OnProgressChanged(view, newProgress);
            }
        }

        protected override void OnDetachedFromWindow()
        {
            //dettached = true;
            if (Control != null)
            {
                Control.StopLoading();
                Control.LoadUrl("about:blank");
            }
            base.OnDetachedFromWindow();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);
            ExtendedWebView = e.NewElement as ExtendedWebView;
            NativeWebView = Control;

            NativeWebView.SetWebViewClient(new ExtendedWebViewClient());
            NativeWebView.SetWebChromeClient(new ExtendedWebViewChrome());

        }
    }
}