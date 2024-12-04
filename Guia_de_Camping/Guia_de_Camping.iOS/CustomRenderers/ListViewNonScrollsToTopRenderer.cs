using Aspbrasil.CustomControls;
using Aspbrasil.iOS.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ListViewNonScrollsToTop), typeof(ListViewNonScrollsToTopRenderer))]
namespace Aspbrasil.iOS.CustomRenderers
{
    public class ListViewNonScrollsToTopRenderer : Xamarin.Forms.Platform.iOS.ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            Control.ScrollsToTop = false;
            Control.AlwaysBounceVertical = false;
        }
    }
}
