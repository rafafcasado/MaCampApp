namespace MaCamp.Dependencias.Permissions
{
    public interface IStoragePermission
    {
        Task<bool> CheckAsync();
        Task<bool> RequestAsync();
    }
}
