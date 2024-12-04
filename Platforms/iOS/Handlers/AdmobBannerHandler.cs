using Google.MobileAds;
using MaCamp.AppSettings;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;
using UIKit;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class AdmobBannerHandler : ViewHandler<AdMobBannerView, BannerView>
    {
        private BannerView? AdView { get; set; }

        protected override BannerView CreatePlatformView()
        {
            AdView = new BannerView
            {
                AdSize = AdSizeCons.Banner,
                AdUnitId = AppConstants.AdmobIdBannerIOs,
                RootViewController = GetRootViewController()
            };

            // Configura o evento de carregamento do anúncio
            AdView.AdReceived += OnAdReceived;

            // Carrega o anúncio
            var request = Request.GetDefaultRequest();
            AdView.LoadRequest(request);

            return AdView;
        }

        private UIViewController GetRootViewController()
        {
            // Obtém o controlador raiz do aplicativo
            var window = UIApplication.SharedApplication.KeyWindow;

            if (window?.RootViewController != null)
            {
                return window.RootViewController;
            }

            throw new InvalidOperationException("Não foi possível localizar o RootViewController.");
        }

        protected override void DisconnectHandler(BannerView platformView)
        {
            // Limpa o banner ao desconectar o handler
            if (AdView != null)
            {
                AdView.AdReceived -= OnAdReceived;
                AdView.RemoveFromSuperview();
                AdView.Dispose();
                AdView = null;
            }

            base.DisconnectHandler(platformView);
        }

        private void OnAdReceived(object? sender, EventArgs e)
        {
            if (AdView != null)
            {
                PlatformView.AddSubview(AdView);
            }
        }
    }
}