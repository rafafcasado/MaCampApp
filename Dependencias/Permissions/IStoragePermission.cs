namespace MaCamp.Dependencias.Permissions
{
    public interface IStoragePermission
    {
        string GetExternalStorageDirectory();

        Task<bool> CheckAsync();
        Task<bool> RequestAsync();
    }
}
