using MaCamp.Dependencias;

namespace MaCamp.Platforms.iOS.Services
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
