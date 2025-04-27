using CoreLocation;
using Foundation;
using MaCamp.Dependencias.Permissions;
using UIKit;

namespace MaCamp.Platforms.iOS.Services.Permissions
{
    public class LocationPermission : ILocationPermission
    {
        public bool IsEnabled()
        {
            return CLLocationManager.LocationServicesEnabled;
        }

        public async Task OpenSettingsAsync()
        {
            var url = new NSUrl(UIKit.UIApplication.OpenSettingsUrlString);

            if (UIApplication.SharedApplication.CanOpenUrl(url))
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }

            await Task.CompletedTask;
        }
    }
}
