using Android.Views;
using AndroidX.Core.View;
using Color = Android.Graphics.Color;
using View = Android.Views.View;

namespace MaCamp.Platforms.Android.Utils
{
    public class InsetsListener : Java.Lang.Object, View.IOnApplyWindowInsetsListener
    {
        public Color? MainColor { get; set; }

        public WindowInsets OnApplyWindowInsets(View view, WindowInsets insets)
        {
            var insetsCompat = WindowInsetsCompat.ToWindowInsetsCompat(insets, view);

            if (insetsCompat != null)
            {
                var statusBarInsets = insetsCompat.GetInsets(WindowInsetsCompat.Type.StatusBars());
                var navigationBarInsets = insetsCompat.GetInsets(WindowInsetsCompat.Type.NavigationBars());

                if (statusBarInsets != null && navigationBarInsets != null)
                {
                    view.SetPadding(0, statusBarInsets.Top, 0, navigationBarInsets.Bottom);
                }

                if (MainColor is Color color)
                {
                    view.SetBackgroundColor(color);
                }
            }

            return insets;
        }
    }
}
