using Foundation;
using MaCamp.Dependencias;
using MaCamp.Utils;
using UserNotifications;

namespace MaCamp.Platforms.iOS.Services
{
    public class NotificationService : INotification
    {
        public NotificationService()
        {
            RequestNotificationPermission();
        }

        private void RequestNotificationPermission()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (granted, error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Erro ao solicitar permissão: {error.LocalizedDescription}");
                }
            });
        }

        public int Show(NotificationData data)
        {
            var id = new Random().Next();

            Update(id, data);

            return id;
        }

        public void Update(int id, NotificationData data)
        {
            var content = new UNMutableNotificationContent
            {
                Title = data.Title,
                Body = data.Message ?? string.Empty,
                Sound = UNNotificationSound.Default,
                Badge = NSNumber.FromInt32(1)
            };

            if (data.ProgressValue.HasValue)
            {
                var progress = data.ProgressValue.Value;

                if (progress >= 0)
                {
                    content.UserInfo = new NSDictionary("progress", progress);
                }
            }

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
            var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    Console.WriteLine($"Erro ao exibir notificação: {error.LocalizedDescription}");
                }
            });
        }

        public void Complete(int id)
        {
            UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new[]
            {
                id.ToString()
            });
        }
    }
}
