using Android.Content;
using Android.Locations;
using Android.Provider;
using MaCamp.Dependencias.Permissions;
using MaCamp.Utils;

namespace MaCamp.Platforms.Android.Services.Permissions
{
    public class LocationPermission : ILocationPermission
    {
        private TaskCompletionSource TaskCompletionSource { get; }

        public LocationPermission()
        {
            TaskCompletionSource = new TaskCompletionSource();
        }

        public bool IsEnabled()
        {
            try
            {
                var service = Platform.AppContext.GetSystemService(Context.LocationService);

                if (service is LocationManager locationManager)
                {
                    return locationManager.IsProviderEnabled(LocationManager.GpsProvider) || locationManager.IsProviderEnabled(LocationManager.NetworkProvider);
                }
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(LocationPermission), nameof(IsEnabled), ex);
            }

            return false;
        }

        public async Task OpenSettingsAsync()
        {
            App.Resumed += OnResumed;

            try
            {
                var intent = new Intent(Settings.ActionLocationSourceSettings);

                intent.SetFlags(ActivityFlags.NewTask);

                Platform.AppContext.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(StoragePermission), nameof(OpenSettingsAsync), ex);

                App.Resumed -= OnResumed;
            }

            // Aguarda o TaskCompletionSource ser completado quando o app retomar
            await TaskCompletionSource.Task;
        }

        private void OnResumed(object? sender, EventArgs e)
        {
            // Quando o app retomar, verifica se a permissão foi concedida
            TaskCompletionSource.TrySetResult();

            App.Resumed -= OnResumed;
        }
    }
}
