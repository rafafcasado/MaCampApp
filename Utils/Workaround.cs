using System.Diagnostics;
using System.Text.Json;
using MaCamp.Dependencias.Permissions;
using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Utils
{
    public static class Workaround
    {
        public static string GetPath()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return Path.Combine("/storage/emulated/0/Documents", AppConstants.NomeApp);
            }

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", AppConstants.NomeApp);
            }

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppConstants.NomeApp);
            }

            if (DeviceInfo.Platform == DevicePlatform.macOS)
            {
                return Path.Combine("Users/Shared", AppConstants.NomeApp);
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static void ShowExceptionOnlyDevolpmentMode(string className, string methodName, Exception? exception, bool showPopup = true)
        {
            var message = $"{className}_{methodName}\n\n{exception?.Message}";

            Debug.WriteLine("-----------<< Exceção >>-----------");
            Debug.WriteLine(message);
            Debug.WriteLine(exception?.StackTrace);

            if (showPopup && Debugger.IsAttached)
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
                await Task.Delay(AppConstants.Delay);

                return await GetServiceAsync<T>();
            }

            var service = provider.Services.GetService<T>();

            if (service != null)
            {
                return service;
            }

            throw new ArgumentNullException(nameof(service), $@"Não foi possível encontrar o serviço {typeof(T).Name}");
        }

        public static async Task TaskWorkAsync(Func<Task> predicate, CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Run(predicate, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Execução cancelada, ignorar erro
                }
                catch (Exception ex)
                {
                    ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskWorkAsync), ex);
                }
            }
        }

        public static async Task<T> TaskWorkAsync<T>(Func<T> predicate, T defaultValue, CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    return await Task.Run(predicate, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Execução cancelada, ignorar erro
                }
                catch (Exception ex)
                {
                    ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                }
            }

            return defaultValue;
        }

        public static async Task TaskWorkAsync(Action action, CancellationToken cancellationToken = default)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Run(action, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Execução cancelada, ignorar erro
                }
                catch (Exception ex)
                {
                    ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                }
            }
        }

        public static async void TaskUI(Action action, CancellationToken cancellationToken = default)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        action();
                    }
                    catch (OperationCanceledException)
                    {
                        // Execução cancelada, ignorar erro
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                    }
                }
            });
        }

        public static async Task TaskUIAsync(Action action)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    action();
                }
                catch (OperationCanceledException)
                {
                    // Execução cancelada, ignorar erro
                }
                catch (Exception ex)
                {
                    ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                }
            });
        }

        public static async Task TaskUIAsync(Func<Task> predicate, CancellationToken cancellationToken = default)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await predicate();
                    }
                    catch (OperationCanceledException)
                    {
                        // Execução cancelada, ignorar erro
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                    }
                }
            });
        }

        public static async Task<T> TaskUIAsync<T>(Func<Task<T>> predicate, T defaultValue, CancellationToken cancellationToken = default)
        {
            return await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        return await predicate();
                    }
                    catch (OperationCanceledException)
                    {
                        // Execução cancelada, ignorar erro
                    }
                    catch (Exception ex)
                    {
                        ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                    }
                }

                return defaultValue;
            });
        }

        public static async Task<Location?> GetLocationAsync(string message, bool openSettings = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var permission = await CheckPermissionAsync<Permissions.LocationWhenInUse>("Localização", message, openSettings);
                var locationService = await GetServiceAsync<ILocationPermission>();
                var locationIsEnabled = locationService.IsEnabled();

                if (permission)
                {
                    if (locationIsEnabled)
                    {
                        var geolocationRequest = new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Best,
                            RequestFullAccuracy = true
                        };
                        var response = await Geolocation.GetLocationAsync(geolocationRequest, cancellationToken);

                        if (AppConstants.ListaDepuracao.Contains(TipoDepuracao.VisualizarGeolocalizacao))
                        {
                            var content = response.ToDictionary().ToString(": ", "\n");
                            var isCopy = await TaskUIAsync(async () => await AppConstants.CurrentPage.DisplayAlert("Localização", content, "Copiar", "Fechar"), false, cancellationToken);

                            if (isCopy)
                            {
                                await Clipboard.SetTextAsync(content);
                            }
                        }

                        return response;
                    }

                    if (openSettings)
                    {
                        var response = await TaskUIAsync(async () => await AppConstants.CurrentPage.DisplayAlert("Localização", AppConstants.Mensagem_Localizacao_Camping, "Habilitar", "Cancelar"), false, cancellationToken);

                        if (response)
                        {
                            await locationService.OpenSettingsAsync();

                            return await GetLocationAsync(message, false, cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(GetLocationAsync), ex);
            }

            return null;
        }

        public static async Task<bool> CheckPermissionAsync<T>(string title, string message, bool showMessage = true) where T : Permissions.BasePermission, new() => await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                try
                {
                    var checkStatus = await Permissions.CheckStatusAsync<T>();

                    if (checkStatus == PermissionStatus.Granted)
                    {
                        return true;
                    }

                    var shouldShowRationale = Permissions.ShouldShowRationale<T>();

                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message) && shouldShowRationale && showMessage)
                    {
                        await TaskUIAsync(async () => await AppConstants.CurrentPage.DisplayAlert(title, message, "OK"));
                    }

                    var requestStatus = await Permissions.RequestAsync<T>();

                    if (requestStatus == PermissionStatus.Granted)
                    {
                        return true;
                    }

                    if (requestStatus != PermissionStatus.Denied && !string.IsNullOrEmpty(title) && showMessage)
                    {
                        await TaskUIAsync(async () => await AppConstants.CurrentPage.DisplayAlert(title, "Não é possível continuar com a ação, tente novamente.", "OK"));
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(CheckPermissionAsync), ex);
                }

                return false;
            }

            return true;
        });

        public static async Task<bool> CheckPermissionAsync(params Permissions.BasePermission[] permissions)
        {
            var results = new List<bool>();

            foreach (var permission in permissions)
            {
                var type = permission.GetType();
                var method = typeof(Workaround).GetMethod(nameof(CheckPermissionAsync))?.MakeGenericMethod(type);

                if (method != null)
                {
                    var parameters = new object[]
                    {
                        string.Empty,
                        string.Empty
                    };
                    var response = method.Invoke(null, parameters);

                    if (response is Task<bool> task)
                    {
                        var result = await task;

                        results.Add(result);
                    }
                }
            }

            return results.All(x => x);
        }

        public static async Task DebounceAsync(string key, int delayMilliseconds, Func<CancellationToken, Task> action)
        {
            if (AppConstants.DictionaryDataDebounceTokens.TryRemove(key, out var existingToken))
            {
                await existingToken.CancelAsync();

                existingToken.Dispose();
            }

            var cancellationTokenSource = new CancellationTokenSource();

            AppConstants.DictionaryDataDebounceTokens[key] = cancellationTokenSource;

            try
            {
                await Task.Delay(delayMilliseconds, cancellationTokenSource.Token);

                if (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await action(cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // Execução foi cancelada antes de completar o tempo do debounce, então ignoramos
            }
            finally
            {
                AppConstants.DictionaryDataDebounceTokens.TryRemove(key, out _);

                cancellationTokenSource.Dispose();
            }
        }

        public static void SerializeWhere<T>(T value, Func<T, bool>? where, Action<string> setter)
        {
            var type = typeof(T);

            try
            {
                if (where == null || where(value))
                {
                    var json = JsonSerializer.Serialize(value, AppConstants.JsonSerializerOptionsDefault);

                    setter(json);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionOnlyDevolpmentMode(nameof(Workaround), $"{nameof(SerializeWhere)}<${type.Name}>", ex);
            }
        }

        public static void Serialize<T>(T value, Action<string> setter) => SerializeWhere(value, null, setter);

        public static T Deserialize<T>(string? json, T defaultValue)
        {
            var type = typeof(T);

            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<T>(json, AppConstants.JsonSerializerOptionsDefault) ?? defaultValue;
                }
            }
            catch (Exception ex)
            {
                ShowExceptionOnlyDevolpmentMode(nameof(Workaround), $"{nameof(Deserialize)}<${type.Name}>", ex);
            }

            return defaultValue;
        }

        public static List<T> DeserializeList<T>(string? json) => Deserialize(json, new List<T>());
    }
}
