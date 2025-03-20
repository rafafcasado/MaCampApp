using MaCamp.Utils;

namespace MaCamp.Dependencias
{
    public interface INotification
    {
        int Show(NotificationData data);
        void Update(int id, NotificationData data);
        void Complete(int id);
    }
}
