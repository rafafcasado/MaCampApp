using Android.Views;
using Color = Android.Graphics.Color;
using View = Android.Views.View;

namespace MaCamp.Platforms.Android.Utils
{
    public class InsetsListener : Java.Lang.Object, View.IOnApplyWindowInsetsListener
    {
        public Color? MainColor { get; set; }
        public Func<bool>? ShouldApplyInsets { get; set; }

        public WindowInsets OnApplyWindowInsets(View view, WindowInsets insets)
        {
            if (ShouldApplyInsets == null || ShouldApplyInsets())
            {
                var topInset = insets.StableInsetTop;
                var bottomInset = insets.StableInsetBottom;

                view.SetPadding(0, topInset, 0, bottomInset);

                if (MainColor is Color color)
                {
                    view.SetBackgroundColor(color);
                }
            }

            return insets;
        }
    }
}
