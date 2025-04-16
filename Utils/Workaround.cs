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

        public static void ShowExceptionOnlyDevolpmentMode(string className, string methodName, Exception? exception)
        {
            var message = $"{className}_{methodName}\n\n{exception?.Message}";

            Debug.WriteLine("-----------<< Exceção >>-----------");
            Debug.WriteLine(message);
            Debug.WriteLine(exception?.StackTrace);

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

        public static void Dispatch(Func<Task> task) => AppConstants.CurrentPage.Dispatcher.Dispatch(async () =>
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(Dispatch), ex);
            }
        });

        public static Task TaskWorkAsync(Func<Task> task, CancellationToken cancellationToken = default)
        {
            try
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception ex)
                    {
                        Workaround.ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskWorkAsync), ex);
                    }
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Execução cancelada, ignorar erro
            }

            return Task.CompletedTask;
        }

        public static async Task TaskUIAsync(Action action, CancellationToken cancellationToken = default)
        {
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            Workaround.ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(TaskUIAsync), ex);
                        }
                    }
                });
            }
            catch (OperationCanceledException)
            {
                // Execução cancelada, ignorar erro
            }
        }

        public static async Task<bool> CheckPermissionAsync<T>(string title, string message) where T : Permissions.BasePermission, new()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                try
                {
                    var checkStatus = await Permissions.CheckStatusAsync<T>();

                    if (checkStatus != PermissionStatus.Granted)
                    {
                        var shouldShowRationale = Permissions.ShouldShowRationale<T>();

                        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message) && shouldShowRationale)
                        {
                            await AppConstants.CurrentPage.DisplayAlert(title, message, "OK");
                        }

                        var requestStatus = await Permissions.RequestAsync<T>();

                        if (requestStatus == PermissionStatus.Granted)
                        {
                            return true;
                        }

                        if (requestStatus != PermissionStatus.Unknown)
                        {
                            if (!string.IsNullOrEmpty(title))
                            {
                                await AppConstants.CurrentPage.DisplayAlert(title, "Não é possível continuar com a ação, tente novamente.", "OK");
                            }

                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowExceptionOnlyDevolpmentMode(nameof(Workaround), nameof(CheckPermissionAsync), ex);

                    return false;
                }
            }

            return true;
        }

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
                existingToken.Cancel();
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
            }
        }
    }
}
