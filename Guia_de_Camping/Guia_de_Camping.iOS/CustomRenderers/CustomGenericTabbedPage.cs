using Aspbrasil.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomGenericTabbedPage))]
namespace Aspbrasil.iOS.CustomRenderers
{
    public class CustomGenericTabbedPage : TabbedRenderer
    {
        public CustomGenericTabbedPage()
        {
            TabBar.Translucent = false;
            TabBar.Opaque = true;

            TabBar.BarTintColor = UIColor.FromRGB(42, 176, 121);
            TabBar.TintColor = UIColor.FromRGB(255, 255, 255);

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                //TabBar.UnselectedItemTintColor = UIColor.FromRGBA(39, 169, 225, 200);
                TabBar.UnselectedItemTintColor = new UIColor(1,1,1, 0.4f);
        }
    }
}