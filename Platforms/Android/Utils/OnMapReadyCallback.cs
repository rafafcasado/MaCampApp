using System.Reflection;
using System.Text.RegularExpressions;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MaCamp.Platforms.Android.Handlers;
using MaCamp.Utils;
using Org.Json;

namespace MaCamp.Platforms.Android.Utils
{
    public class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
    {
        private CustomGoogleMapsMapHandler MapHandler { get; }
        private Dictionary<string, BitmapDescriptor> DictionaryImages { get; }

        public OnMapReadyCallback(CustomGoogleMapsMapHandler mapHandler)
        {
            var assembly = typeof(OnMapReadyCallback).Assembly;
            var listResourceNames = assembly.GetManifestResourceNames();

            MapHandler = mapHandler;
            DictionaryImages = listResourceNames.Where(x => x.EndsWith("_small.png")).ToDictionary(x => Regex.Replace(x, @"^.*(?=\bpointer_)", string.Empty), x => assembly.GetManifestResourceBitmapDescriptor(x));
        }

        public void OnMapReady(GoogleMap nativeMap)
        {
            var json = MapHandler.VirtualView.GeoJson;

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var geojson = new JSONObject(json);
                    var listFeatures = geojson.ToList("features");

                    foreach (var feature in listFeatures)
                    {
                        var markerOpts = new MarkerOptions();
                        var properties = feature.GetJSONObject("properties");
                        var geometry = feature.GetJSONObject("geometry");

                        if (properties.TryGetValue<string>("title", out var title))
                        {
                            markerOpts.SetTitle(title);
                        }
                        if (properties.TryGetValue<string>("snippet", out var snippet))
                        {
                            markerOpts.SetSnippet(snippet);
                        }
                        if (properties.TryGetValue<string>("icon", out var icon) && DictionaryImages.TryGetValue<string, BitmapDescriptor>(icon, out var bitmapDescriptor))
                        {
                            markerOpts.SetIcon(bitmapDescriptor);
                        }
                        if (geometry.TryGetValue<(double, double)>("coordinates", out var coordinates))
                        {
                            var (lat, lon) = coordinates;

                            markerOpts.SetPosition(new LatLng(lat, lon));
                        }
                    }

                    nativeMap.SetOnInfoWindowClickListener(new CustomInfoWindowClickListener(MapHandler));
                }
                catch (Exception ex)
                {
                    Workaround.ShowExceptionOnlyDevolpmentMode(nameof(OnMapReadyCallback), nameof(OnMapReady), ex);
                }
            }
        }
    }

    public class CustomInfoWindowClickListener : Java.Lang.Object, GoogleMap.IOnInfoWindowClickListener
    {
        private CustomGoogleMapsMapHandler MapHandler { get; }
        private MethodInfo? OnInfoWindowClickMethod { get; }

        public CustomInfoWindowClickListener(CustomGoogleMapsMapHandler mapHandler)
        {
            MapHandler = mapHandler;
            OnInfoWindowClickMethod = typeof(MPowerKit.GoogleMaps.GoogleMap).GetMethod("SendInfoWindowClick");
        }

        public void OnInfoWindowClick(Marker marker)
        {
            var pin = MapHandler.VirtualView.Pins.FirstOrDefault(p => marker.Tag?.ToString() is string tag && p.ClassId == tag);

            if (OnInfoWindowClickMethod != null && MapHandler.VirtualView is MPowerKit.GoogleMaps.GoogleMap googleMap)
            {
                OnInfoWindowClickMethod.Invoke(googleMap, new object[]
                {
                    pin
                });
            }
        }
    }
}
