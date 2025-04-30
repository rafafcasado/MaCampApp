using Android.Gms.Maps;
using MaCamp.Platforms.Android.Utils;
using Maui.GoogleMaps.Handlers;

namespace MaCamp.Platforms.Android.Handlers
{
    public class CustomMapHandler : MapHandler
    {
        protected override MapView CreatePlatformView()
        {
            var mapView = new MapView(Context);

            mapView.OnCreate(null);
            mapView.OnResume();
            mapView.GetMapAsync(new OnMapReadyCallback(VirtualView));

            return mapView;
        }
    }
}
