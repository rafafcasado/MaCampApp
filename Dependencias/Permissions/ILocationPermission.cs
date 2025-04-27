
namespace MaCamp.Dependencias.Permissions
{
    public interface ILocationPermission
    {
        bool IsEnabled();
        Task OpenSettingsAsync();
    }
}
