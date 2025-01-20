using CoreGraphics;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;
using UIKit;

namespace MaCamp.Platforms.iOS.Handlers
{
    public partial class IconViewHandler : ViewHandler<IconView, UIImageView>
    {
        private static IPropertyMapper<IconView, IconViewHandler> Mapper => new PropertyMapper<IconView, IconViewHandler>(ViewMapper)
        {
            [nameof(Microsoft.Maui.Controls.WebView.Source)] = MapSource
        };
        private bool IsDisposed { get; set; }

        public IconViewHandler() : base(Mapper)
        {
        }

        protected override UIImageView CreatePlatformView()
        {
            return new UIImageView(CGRect.Empty)
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                ClipsToBounds = true
            };
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

        private static void MapSource(IconViewHandler handler, IconView iconView)
        {
            var image = UIImage.FromBundle(iconView.Source);

            handler.PlatformView.Image = image;
        }
    }
}