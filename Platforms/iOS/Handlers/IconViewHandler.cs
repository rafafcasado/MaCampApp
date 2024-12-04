using CoreGraphics;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;
using UIKit;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class IconViewHandler : ViewHandler<IconView, UIImageView>
    {
        private bool IsDisposed { get; set; }

        protected override UIImageView CreatePlatformView()
        {
            return new UIImageView(CGRect.Empty)
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true
            };
        }

        public partial void MapSource(IconViewHandler handler, IconView iconView)
        {
            var image = UIImage.FromBundle(iconView.Source);
            handler.PlatformView.Image = image;
        }

        protected override void DisconnectHandler(UIImageView nativeView)
        {
            base.DisconnectHandler(nativeView);

            // Limpeza de recursos quando o handler é desconectado
            if (nativeView.Image != null)
            {
                nativeView.Image.Dispose();
                nativeView.Image = null;
            }
        }
    }
}