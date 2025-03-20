namespace MaCamp.Dependencias
{
    public interface IStoragePermission
    {
        Task<bool> Check();
        Task<bool> Request();
    }
}
