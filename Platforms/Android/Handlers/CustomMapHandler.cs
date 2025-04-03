using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using MaCamp.CustomControls;
using MaCamp.Utils;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Platform;
using SkiaSharp;
using IMap = Microsoft.Maui.Maps.IMap;
using Path = System.IO.Path;

namespace MaCamp.Platforms.Android.Handlers
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

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);

            var mapReady = new MapCallbackHandler(this);

            platformView.Clickable = true;

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
            if (Map != null && MauiContext != null)
            {
                foreach (var pin in mapPins)
                {
                    var pinHandler = pin.ToHandler(MauiContext);

                    if (pinHandler is IMapPinHandler mapPinHandler)
                    {
                        var markerOption = mapPinHandler.PlatformView;

                        if (pin is StylishPin stylishPin)
                        {
                            if (stylishPin.ImageSource == null || stylishPin.ImageSource.IsEmpty)
                            {
                                var bitmapDescriptor = CreateBitmapDynamic(stylishPin.Label);

                                markerOption.SetIcon(bitmapDescriptor);

                                AddMarker(Map, pin, Markers, markerOption);
                            }
                            else if (stylishPin.ImageSource is FileImageSource fileImageSource)
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
        }

        private void AddMarker(GoogleMap map, IMapPin pin, List<Marker> markers, MarkerOptions markerOption)
        {
            var marker = map.AddMarker(markerOption);

            pin.MarkerId = marker.Id;

            markers.Add(marker);
        }

        private Bitmap GetMaximumBitmap(Bitmap? sourceImage, float maxWidth, float maxHeight)
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

        private string? CreateImageDynamicPath(string text)
        {
            try
            {
                var normalizeText = text.NormalizeText();
                var fileName = $"{normalizeText}.png";
                var directoryPath = Path.Combine(Workaround.GetPath(), "images");
                var filePath = Path.Combine(directoryPath, fileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (File.Exists(filePath))
                {
                    return filePath;
                }

                var paint = new SKPaint
                {
                    Color = AppColors.CorPrimaria.ToSKColor(),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };
                var textSize = 40 * (1 - (10 * text.Length) / 100);
                var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = textSize,
                    TextAlign = SKTextAlign.Center,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                };
                using var bitmap = new SKBitmap(100, 100);
                using var canvas = new SKCanvas(bitmap);

                canvas.Clear(SKColors.Transparent);
                canvas.DrawCircle(50, 50, 40, paint);
                canvas.DrawText(text.ToString(), 50, 60, textPaint);

                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                using var stream = File.OpenWrite(filePath);

                data.SaveTo(stream);

                return filePath;
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(CustomMapHandler), nameof(CreateImageDynamicPath), ex);
            }

            return null;
        }

        private BitmapDescriptor CreateBitmapDynamic(string text)
        {
            try
            {
                var size = 50 + 10 * text.Length;
                using var surface = SKSurface.Create(new SKImageInfo(size, size));
                var canvas = surface.Canvas;

                canvas.Clear(SKColors.Transparent);

                var circleSize = Convert.ToSingle(size * 0.5);
                using var paintBackground = new SKPaint
                {
                    Color = AppColors.CorPrimaria.ToSKColor(),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawCircle(circleSize, circleSize, circleSize, paintBackground);

                var textSize = 30 * (1 - (10 * text.Length) / 100);
                using var textPaint = new SKPaint
                {
                    Color = SKColors.White,
                    TextSize = textSize,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                };

                var textBounds = new SKRect();

                textPaint.MeasureText(text, ref textBounds);

                var xText = size / 2;
                var yText = (size / 2) - textBounds.MidY;

                canvas.DrawText(text, xText, yText, textPaint);

                using var image = surface.Snapshot();
                using var data = image.Encode();
                using var stream = data.AsStream();
                var bitmap = BitmapFactory.DecodeStream(stream);

                return BitmapDescriptorFactory.FromBitmap(bitmap);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(CustomMapHandler), nameof(CreateBitmapDynamic), ex);
            }

            return BitmapDescriptorFactory.DefaultMarker();
        }
    }
}