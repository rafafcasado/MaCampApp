using Aspbrasil.iOS.CustomRenderers;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Frame), typeof(ExtendedFrameRenderer))]
namespace Aspbrasil.iOS.CustomRenderers
{
    public class ExtendedFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);
            var elem = (Frame)this.Element;
            if (elem != null)
            {

                // Border
                this.Layer.CornerRadius = (float)elem.CornerRadius;
                //this.Layer.Bounds.Inset((int)elem.BorderThickness, (int)elem.BorderThickness);
                Layer.BorderColor = elem.BorderColor.ToCGColor();
                Layer.BorderWidth = (float)2;
                //Layer.BorderWidth = (float)elem.BorderThickness;

                // Shadow
                this.Layer.ShadowColor = UIColor.DarkGray.CGColor;
                this.Layer.ShadowOpacity = 0.6f;
                this.Layer.ShadowRadius = 1.0f;
                this.Layer.ShadowOffset = new SizeF(0, 0);
                //this.Layer.MasksToBounds = true;
            }
        }

    }
}