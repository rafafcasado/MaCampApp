using System.Diagnostics;

namespace MaCamp.Utils
{
    public static class Workaround
    {
        public static string GetPath()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return Path.Combine("/storage/emulated/0/Documents", "MaCamp");
            }

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "MaCamp");
            }

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MaCamp");
            }

            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                return Path.Combine("Users/Shared/MaCamp");
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static void ShowExceptionOnlyDevolpmentMode(string className, string methodName, Exception exception)
        {
            var message = $"{className}_{methodName}\n\n{exception.Message}";

            Debug.WriteLine("-----------<< Exceção >>-----------");
            Debug.WriteLine(message);
            Debug.WriteLine(exception.StackTrace);

            if (Debugger.IsAttached)
            {
                AppConstants.CurrentPage.Dispatcher.Dispatch(async () =>
                {
                    await AppConstants.CurrentPage.DisplayAlert("Mensagem somente em DEV", message, "OK");
                });
            }
        }

        public static async Task<T> GetServiceAsync<T>()
        {
            var provider = AppConstants.CurrentPage.Handler?.MauiContext;

            if (provider == null)
            {
                await Task.Delay(250);

                return await GetServiceAsync<T>();
            }

            var service = provider.Services.GetService<T>();

            if (service != null)
            {
                return service;
            }

            throw new ArgumentNullException(nameof(service), $@"Não foi possível encontrar o serviço {typeof(T).Name}");
        }
    }
}
