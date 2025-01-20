using Android.Widget;
using MaCamp.CustomControls;
using Microsoft.Maui.Handlers;

namespace MaCamp.Platforms.Android.Handlers
{
    public partial class IconViewHandler : ViewHandler<IconView, ImageView>
    {
        private static IPropertyMapper<IconView, IconViewHandler> Mapper => new PropertyMapper<IconView, IconViewHandler>(ViewMapper)
        {
            [nameof(Microsoft.Maui.Controls.WebView.Source)] = MapSource
        };

        public IconViewHandler() : base(Mapper)
        {
        }

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

        private static void MapSource(IconViewHandler handler, IconView iconView)
        {
            handler.PlatformView.SetImageResource(handler.PlatformView.SourceLayoutResId);
        }
    }
}