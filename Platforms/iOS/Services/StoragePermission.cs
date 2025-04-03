using MaCamp.Dependencias;

namespace MaCamp.Platforms.iOS.Services
{
    public class StoragePermission : IStoragePermission
    {
        public Task<bool> Check()
        {
            return Task.FromResult(true);
        }

        public Task<bool> Request()
        {
            return Task.FromResult(true);
        }
    }
}
