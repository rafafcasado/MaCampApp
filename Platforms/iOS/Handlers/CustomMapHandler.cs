using CoreLocation;
using MaCamp.CustomControls;
using MaCamp.Platforms.iOS.Utils;
using MapKit;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps.Platform;
using Microsoft.Maui.Platform;
using UIKit;
using IMap = Microsoft.Maui.Maps.IMap;

namespace MaCamp.Platforms.iOS.Handlers
{
    public class CustomMapHandler : MapHandler
    {
        public static readonly IPropertyMapper<IMap, IMapHandler> CustomMapper = new PropertyMapper<IMap, IMapHandler>(Mapper)
        {
            [nameof(IMap.Pins)] = MapPins,
        };

        public List<IMKAnnotation> Markers { get; }
        private static UIView? LastTouchedView { get; set; }

        public CustomMapHandler() : base(CustomMapper, CommandMapper)
        {
            Markers = new List<IMKAnnotation>();
        }

        public CustomMapHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null) : base(mapper ?? CustomMapper, commandMapper ?? CommandMapper)
        {
            Markers = new List<IMKAnnotation>();
        }

        protected override void ConnectHandler(MauiMKMapView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.GetViewForAnnotation += GetViewForAnnotations;
        }

        private static void OnCalloutClicked(IMKAnnotation annotation)
        {
            var pin = GetPinForAnnotation(annotation);

            if (LastTouchedView is MKAnnotationView)
            {
                return;
            }

            pin?.SendInfoWindowClick();
        }

        private static MKAnnotationView GetViewForAnnotations(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView;

            if (annotation is CustomAnnotation customAnnotation)
            {
                annotationView = mapView.DequeueReusableAnnotation(customAnnotation.Identifier.ToString()) ?? new MKAnnotationView(annotation, customAnnotation.Identifier.ToString());
                annotationView.Image = customAnnotation.Image;
                annotationView.CanShowCallout = true;
            }
            else if (annotation is MKPointAnnotation)
            {
                annotationView = mapView.DequeueReusableAnnotation("defaultPin") ?? new MKMarkerAnnotationView(annotation, "defaultPin");
                annotationView.CanShowCallout = true;
            }
            else
            {
                annotationView = new MKUserLocationView(annotation, null);
            }

            var result = annotationView ?? new MKAnnotationView(annotation, null);

            AttachGestureToPin(result, annotation);

            return result;
        }

        private static void AttachGestureToPin(MKAnnotationView mapPin, IMKAnnotation annotation)
        {
            var recognizers = mapPin.GestureRecognizers;

            if (recognizers != null)
            {
                foreach (var r in recognizers)
                {
                    mapPin.RemoveGestureRecognizer(r);
                }
            }
            var recognizer = new UITapGestureRecognizer(x => OnCalloutClicked(annotation))
            {
                ShouldReceiveTouch = (gestureRecognizer, touch) =>
                {
                    LastTouchedView = touch.View;
                    return true;
                }
            };

            mapPin.AddGestureRecognizer(recognizer);
        }

        private static IMapPin? GetPinForAnnotation(IMKAnnotation? annotation)
        {
            if (annotation is CustomAnnotation customAnnotation)
            {
                return customAnnotation.Pin;
            }

            return null;
        }

        private static new void MapPins(IMapHandler handler, IMap map)
        {
            if (handler is CustomMapHandler mapHandler)
            {
                foreach (var marker in mapHandler.Markers)
                {
                    mapHandler.PlatformView.RemoveAnnotation(marker);
                }

                mapHandler.Markers.Clear();
                mapHandler.AddPins(map.Pins);
            }
        }

        private void AddPins(IEnumerable<IMapPin> mapPins)
        {
            if (MauiContext is null)
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
                        stylishPin.ImageSource.LoadImage(MauiContext, result =>
                        {
                            markerOption = new CustomAnnotation()
                            {
                                Identifier = stylishPin.Id,
                                Image = result?.Value,
                                Title = pin.Label,
                                Subtitle = pin.Address,
                                Coordinate = new CLLocationCoordinate2D(pin.Location.Latitude, pin.Location.Longitude),
                                Pin = stylishPin
                            };

                            AddMarker(PlatformView, pin, Markers, markerOption);
                        });
                    }
                    else
                    {
                        AddMarker(PlatformView, pin, Markers, markerOption);
                    }
                }
            }
        }

        private static void AddMarker(MauiMKMapView map, IMapPin pin, List<IMKAnnotation> markers, IMKAnnotation annotation)
        {
            map.AddAnnotation(annotation);

            pin.MarkerId = annotation;

            markers.Add(annotation);
        }
    }

}