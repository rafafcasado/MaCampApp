using Android.Gms.Maps;
using MPowerKit.GoogleMaps;

namespace MaCamp.Platforms.Android.Handlers
{
    public class CustomGoogleMapsMapHandler : GoogleMapHandler
    {
        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);

            //PlatformView.GetMapAsync(new MaCamp.Platforms.Android.Utils.OnMapReadyCallback(this));
        }
    }
}
