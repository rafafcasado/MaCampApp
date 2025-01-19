using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using MaCamp.CustomControls;
using MaCamp.Platforms.Android.Handlers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using IMap = Microsoft.Maui.Maps.IMap;

// ReSharper disable once CheckNamespace
namespace MaCamp.Handlers
{
    public class CustomMapHandler : MapHandler
    {
        public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper = new PropertyMapper<IMap, IMapHandler>(Mapper)
        {
            [nameof(IMap.Pins)] = MapPins,
        };

        public List<Marker> Markers { get; }

        public CustomMapHandler() : base(CustomMapper, CommandMapper)
        {
            Markers = new List<Marker>();
        }

        public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(mapper ?? CustomMapper, commandMapper ?? CommandMapper)
        {
            Markers = new List<Marker>();
        }

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);

            var mapReady = new MapCallbackHandler(this);

            PlatformView.GetMapAsync(mapReady);
        }

        private static new void MapPins(IMapHandler handler, IMap map)
        {
            if (handler is CustomMapHandler mapHandler)
            {
                foreach (var marker in mapHandler.Markers)
                {
                    marker.Remove();
                }

                mapHandler.AddPins(map.Pins);
            }
        }

        private void AddPins(IEnumerable<IMapPin> mapPins)
        {
            if (Map is null || MauiContext is null)
            {
                return;
            }

            foreach (var pin in mapPins)
            {
                var pinHandler = pin.ToHandler(MauiContext);

                if (pinHandler is IMapPinHandler mapPinHandler)
                {
                    var markerOption = mapPinHandler.PlatformView;

                    if (pin is StylishPin stylishPin)
                    {
                        if (stylishPin.ImageSource is FileImageSource fileImageSource)
                        {
                            var bitmapDescriptor = BitmapDescriptorFactory.FromAsset(fileImageSource.File);

                            markerOption.SetIcon(bitmapDescriptor);

                            AddMarker(Map, pin, Markers, markerOption);
                        }
                        else
                        {
                            stylishPin.ImageSource.LoadImage(MauiContext, result =>
                            {
                                if (result?.Value is BitmapDrawable bitmapDrawable)
                                {
                                    markerOption.SetIcon(BitmapDescriptorFactory.FromBitmap(GetMaximumBitmap(bitmapDrawable.Bitmap, 100, 100)));
                                }

                                AddMarker(Map, pin, Markers, markerOption);
                            });
                        }
                    }
                    else
                    {
                        AddMarker(Map, pin, Markers, markerOption);
                    }
                }
            }
        }

        private static void AddMarker(GoogleMap map, IMapPin pin, List<Marker> markers, MarkerOptions markerOption)
        {
            var marker = map.AddMarker(markerOption);

            pin.MarkerId = marker.Id;

            markers.Add(marker);
        }

        private static Bitmap GetMaximumBitmap(Bitmap? sourceImage, float maxWidth, float maxHeight)
        {
            if (sourceImage != null)
            {
                var sourceSize = new Size(sourceImage.Width, sourceImage.Height);
                var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
                var width = Math.Max(maxResizeFactor * sourceSize.Width, 1);
                var height = Math.Max(maxResizeFactor * sourceSize.Height, 1);
                var result = Bitmap.CreateScaledBitmap(sourceImage, Convert.ToInt32(width), Convert.ToInt32(height), false);

                if (result != null)
                {
                    return result;
                }
            }

            throw new InvalidOperationException("Failed to create Bitmap");
        }
    }
}