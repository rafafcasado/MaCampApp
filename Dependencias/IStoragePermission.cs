namespace MaCamp.Dependencias
{
    public interface IStoragePermission
    {
        Task<bool> CheckAsync();
        Task<bool> RequestAsync();
    }
}
