using Android.Content;
using Android.OS;
using MaCamp.Dependencias.Permissions;
using MaCamp.Utils;
using Environment = Android.OS.Environment;
using Settings = Android.Provider.Settings;
using Uri = Android.Net.Uri;

namespace MaCamp.Platforms.Android.Services.Permissions
{
    public class StoragePermission : IStoragePermission
    {
        private TaskCompletionSource<bool> TaskCompletionSource { get; }

        public StoragePermission()
        {
            TaskCompletionSource = new TaskCompletionSource<bool>();
        }

        public string GetExternalStorageDirectory()
        {
            var file = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDocuments);

            if (file != null)
            {
                return file.AbsolutePath;
            }

            return "/storage/emulated/0/Documents";
        }

        public async Task<bool> CheckAsync()
        {
            // Somente necessário no Android 11+
            if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(Environment.IsExternalStorageManager);
        }

        public async Task<bool> RequestAsync()
        {
            // Somente necessário no Android 11+
            if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                return true;
            }

            // Já tem permissão
            if (Environment.IsExternalStorageManager)
            {
                return true;
            }

            App.Resumed += OnResumed;

            try
            {
                var uri = Uri.Parse("package:" + AppInfo.PackageName);
                var intent = new Intent(Settings.ActionManageAppAllFilesAccessPermission, uri);

                intent.SetFlags(ActivityFlags.NewTask);

                Platform.AppContext.StartActivity(intent);
            }
            catch (Exception ex)
            {
                Workaround.ShowExceptionOnlyDevolpmentMode(nameof(StoragePermission), nameof(RequestAsync), ex);

                App.Resumed -= OnResumed;

                return false;
            }

            // Aguarda o TaskCompletionSource ser completado quando o app retomar
            var permissionGranted = await TaskCompletionSource.Task;

            return permissionGranted;
        }

        private void OnResumed(object? sender, EventArgs e)
        {
            // Quando o app retomar, verifica se a permissão foi concedida
            TaskCompletionSource.TrySetResult(Environment.IsExternalStorageManager);

            App.Resumed -= OnResumed;
        }
    }
}
