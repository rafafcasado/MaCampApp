namespace MaCamp.Dependencias
{
    public interface IStoragePermission
    {
        Task<bool> Request();
    }
}
