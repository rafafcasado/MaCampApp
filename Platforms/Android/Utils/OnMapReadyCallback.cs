using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MaCamp.Utils;
using Maui.GoogleMaps;
using Maui.GoogleMaps.Clustering;
using BitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using Map = Maui.GoogleMaps.Map;

namespace MaCamp.Platforms.Android.Utils
{
    public class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback, GoogleMap.IOnInfoWindowClickListener
    {
        private ClusteredMap? ClusteredMap { get; }
        private Dictionary<string, BitmapDescriptor> DictionaryImages { get; }

        public OnMapReadyCallback(Map map)
        {
            var assembly = typeof(OnMapReadyCallback).Assembly;
            var listResourceNames = assembly.GetManifestResourceNames();

            DictionaryImages = listResourceNames.Where(x => x.EndsWith("_small.png")).ToDictionary(x => Regex.Replace(x, @"^.*(?=\bpointer_)", string.Empty), x => assembly.GetManifestResourceBitmapDescriptor(x));

            if (map is ClusteredMap clusteredMap)
            {
                ClusteredMap = clusteredMap;
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            if (ClusteredMap != null)
            {
                googleMap.SetOnInfoWindowClickListener(this);

                if (!string.IsNullOrEmpty(ClusteredMap.GeoJson))
                {
                    var geoJson = JsonNode.Parse(ClusteredMap.GeoJson);

                    if (geoJson.TryGetValue<JsonArray>("features", out var features))
                    {
                        Parallel.ForEach(features, async feature =>
                        {
                            if (feature.TryGetValue<JsonNode>("geometry", out var geometry) && geometry.TryGetValue<string>("type", out var type) && type == "Point" && feature.TryGetValue<JsonNode>("properties", out var properties))
                            {
                                if (geometry.TryGetValue<JsonArray>("coordinates", out var coordinates) && coordinates.Count == 2)
                                {
                                    if (coordinates.TryGetValue<double>(0, out var longitude) && coordinates.TryGetValue<double>(1, out var latitude) && properties.TryGetValue<string>("title", out var title) && properties.TryGetValue<string>("snippet", out var snippet) && properties.TryGetValue<string>("icon", out var icon))
                                    {
                                        try
                                        {
                                            var markerOptions = new MarkerOptions();

                                            markerOptions.SetPosition(new LatLng(latitude, longitude));
                                            markerOptions.SetTitle(title);
                                            markerOptions.SetSnippet(snippet);

                                            if (DictionaryImages.TryGetValue<string, BitmapDescriptor>(icon, out var bitmapDescriptor))
                                            {
                                                markerOptions.SetIcon(bitmapDescriptor);
                                            }

                                            await MainThread.InvokeOnMainThreadAsync(() =>
                                            {
                                                googleMap.AddMarker(markerOptions);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Workaround.ShowExceptionOnlyDevolpmentMode(nameof(OnMapReady), nameof(OnMapReady), ex);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }
        }

        public void OnInfoWindowClick(Marker marker)
        {
            if (ClusteredMap != null)
            {
                var pin = new Pin
                {
                    Label = marker.Title,
                    Address = marker.Snippet,
                    Position = new Position(marker.Position.Latitude, marker.Position.Longitude)
                };

                ClusteredMap.SendInfoWindowClicked(pin);
            }
        }
    }
}
