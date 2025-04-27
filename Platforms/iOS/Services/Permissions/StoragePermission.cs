using MaCamp.Dependencias.Permissions;

namespace MaCamp.Platforms.iOS.Services.Permissions
{
    public class StoragePermission : IStoragePermission
    {
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
