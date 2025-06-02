namespace MaCamp.Dependencias.Permissions
{
    public interface IStoragePermission
    {
        string GetExternalDirectory();
        string GetInternalDirectory();

        Task<bool> CheckAsync();
        Task<bool> RequestExternalPermissionAsync();
    }
}
