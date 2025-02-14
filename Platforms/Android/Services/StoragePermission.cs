using Android.Content;
using Android.OS;
using MaCamp.Dependencias;
using Environment = Android.OS.Environment;
using Settings = Android.Provider.Settings;
using Uri = Android.Net.Uri;

namespace MaCamp.Platforms.Android.Services
{
    public class StoragePermission : IStoragePermission
    {
        public async Task<bool> Request()
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

            try
            {
                var uri = Uri.Parse("package:" + AppInfo.PackageName);
                var intent = new Intent(Settings.ActionManageAppAllFilesAccessPermission, uri);

                if (Platform.CurrentActivity != null)
                {
                    Platform.CurrentActivity.StartActivity(intent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao solicitar MANAGE_EXTERNAL_STORAGE: " + ex.Message);

                return false;
            }

            // Espera um pequeno tempo para a permissão ser concedida
            await Task.Delay(3000);

            return Environment.IsExternalStorageManager;
        }
    }
}
