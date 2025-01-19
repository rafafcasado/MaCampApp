using Android.Gms.Maps;
using Microsoft.Maui.Maps.Handlers;
using IMap = Microsoft.Maui.Maps.IMap;

namespace MaCamp.Platforms.Android.Handlers
{
    public class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback
    {
        private IMapHandler MapHandler { get; }

        public MapCallbackHandler(IMapHandler mapHandler)
        {
            MapHandler = mapHandler;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            MapHandler.UpdateValue(nameof(IMap.Pins));
        }
    }
}
