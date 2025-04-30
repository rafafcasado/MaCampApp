using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Maui.GoogleMaps.Clustering;
using Map = Maui.GoogleMaps.Map;

namespace MaCamp.Platforms.Android.Utils
{
    public class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
    {
        private ClusteredMap? ClusteredMap { get; }
        private Dictionary<string, Stream?> DictionaryImages { get; }

        public OnMapReadyCallback(Map map)
        {
            var assembly = typeof(OnMapReadyCallback).Assembly;
            var listResourceNames = assembly.GetManifestResourceNames();

            DictionaryImages = listResourceNames.Where(x => x.EndsWith("_small.png")).ToDictionary(x => Regex.Replace(x, @"^.*(?=\bpointer_)", string.Empty), x => assembly.GetManifestResourceStream(x));

            if (map is ClusteredMap clusteredMap)
            {
                ClusteredMap = clusteredMap;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            if (ClusteredMap != null && !string.IsNullOrEmpty(ClusteredMap.GeoJson))
            {
                var geoJson = JsonNode.Parse(ClusteredMap.GeoJson);
                var features = geoJson?["features"]?.AsArray();

                if (features != null)
                {
                    foreach (var feature in features)
                    {
                        var geometry = feature?["geometry"];
                        var properties = feature?["properties"];

                        if (geometry?["type"]?.ToString() == "Point")
                        {
                            var coordinates = geometry["coordinates"]?.AsArray();
                            if (coordinates != null && coordinates.Count == 2)
                            {
                                var longitude = coordinates[0]?.GetValue<double>() ?? 0;
                                var latitude = coordinates[1]?.GetValue<double>() ?? 0;
                                var title = properties?["title"]?.ToString() ?? string.Empty;
                                var snippet = properties?["snippet"]?.ToString() ?? string.Empty;
                                var iconName = properties?["icon"]?.ToString() ?? string.Empty;

                                if (!string.IsNullOrEmpty(iconName) && DictionaryImages.TryGetValue(iconName, out var stream) && stream != null && stream != Stream.Null)
                                {
                                    var markerOptions = new MarkerOptions().SetPosition(new LatLng(latitude, longitude)).SetTitle(title).SetSnippet(snippet);
                                    var bitmap = BitmapFactory.DecodeStream(stream);

                                    if (bitmap != null)
                                    {
                                        var descriptor = BitmapDescriptorFactory.FromBitmap(bitmap);

                                        markerOptions.SetIcon(descriptor);
                                    }

                                    googleMap.AddMarker(markerOptions);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
