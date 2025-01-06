using Android.Views;
using View = Android.Views.View;

namespace MaCamp.Platforms.Android.Utils
{
    public class InsetsListener : Java.Lang.Object, View.IOnApplyWindowInsetsListener
    {
        private Func<bool> ShouldApplyInsets { get; }

        public InsetsListener(Func<bool> shouldApplyInsets)
        {
            ShouldApplyInsets = shouldApplyInsets;
        }

        public WindowInsets OnApplyWindowInsets(View view, WindowInsets insets)
        {
            if (ShouldApplyInsets())
            {
                var topInset = insets.StableInsetTop;
                var bottomInset = insets.StableInsetBottom;

                view.SetPadding(0, topInset, 0, bottomInset);
            }

            return insets;
        }
    }
}
