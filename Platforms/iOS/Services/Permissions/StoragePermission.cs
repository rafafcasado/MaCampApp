using Foundation;
using MaCamp.Dependencias.Permissions;

namespace MaCamp.Platforms.iOS.Services.Permissions
{
    public class StoragePermission : IStoragePermission
    {
        public string GetExternalStorageDirectory()
        {
            var listUrls = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
            var url = listUrls.FirstOrDefault();

            if (url != null && !string.IsNullOrEmpty(url.Path))
            {
                return url.Path;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public Task<bool> CheckAsync()
        {
            return Task.FromResult(true);
        }

        public Task<bool> RequestAsync()
        {
            return Task.FromResult(true);
        }
    }
}
