using Android.Widget;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public partial class IconViewHandler : ViewHandler<IconView, ImageView>
    {
        protected override ImageView CreatePlatformView()
        {
            return new ImageView(Context);
        }

        protected override void ConnectHandler(ImageView platformView)
        {
            base.ConnectHandler(platformView);
            var drawable = Context.GetDrawable(platformView.SourceLayoutResId)?.Mutate();

            if (drawable != null && platformView.Foreground != null)
            {
                drawable.SetColorFilter(platformView.Foreground.ColorFilter);
                drawable.SetAlpha((int)platformView.Alpha);
                PlatformView.SetImageDrawable(drawable);
            }
        }

        public partial void MapSource(IconViewHandler handler, IconView iconView)
        {
            handler.PlatformView.SetImageResource(handler.PlatformView.SourceLayoutResId);
        }
    }
}