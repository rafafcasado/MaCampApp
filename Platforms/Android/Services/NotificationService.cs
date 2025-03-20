using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using MaCamp.Dependencias;
using MaCamp.Utils;

namespace MaCamp.Platforms.Android.Services
{
    public class NotificationService : INotification
    {
        private Context Context { get; }
        private Random Randomizer { get; }
        private NotificationManager? NotificationManager { get; }

        public NotificationService()
        {
            var context = Platform.AppContext;

            Context = context;
            Randomizer = new Random();

            if (context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager)
            {
                NotificationManager = notificationManager;
            }

            CreateNotificationChannel();
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(AppConstants.NotificationManager_ChannelId, AppConstants.NotificationManager_ChannelName, NotificationImportance.High)
                {
                    Blockable = true,
                    Description = AppConstants.NotificationManager_ChannelDescription,
                    LockscreenVisibility = NotificationVisibility.Public,
                };

                if (NotificationManager != null)
                {
                    NotificationManager.CreateNotificationChannel(channel);
                }
            }
        }

        public int Show(NotificationData data)
        {
            var id = Randomizer.Next();

            Update(id, data);

            return id;
        }

        public void Update(int id, NotificationData data)
        {
            var notification = BuildNotification(data);

            if (NotificationManager != null)
            {
                NotificationManager.Notify(id, notification);
            }
        }

        public void Complete(int id)
        {
            if (NotificationManager != null)
            {
                NotificationManager.Cancel(id);
            }
        }

        private Notification BuildNotification(NotificationData data)
        {
            var builder = new NotificationCompat.Builder(Context, AppConstants.NotificationManager_ChannelId);

            builder.SetContentTitle(data.Title);
            builder.SetContentText(data.Message);
            builder.SetSmallIcon(Resource.Drawable.icone_aba1);
            builder.SetPriority(NotificationCompat.PriorityHigh);
            builder.SetOngoing(data.ProgressValue.HasValue);
            builder.SetAutoCancel(!data.ProgressValue.HasValue);
            builder.SetOnlyAlertOnce(true);
            builder.SetSilent(true);

            var maxProgress = data.ProgressValue.HasValue && data.ProgressValue != 0 ? 100 : 0;
            var currentProgress = data.ProgressValue.HasValue && data.ProgressValue != 0 ? data.ProgressValue.Value : 0;
            var isIndeterminate = data.ProgressValue.HasValue && data.ProgressValue.Value < 0;

            builder.SetProgress(maxProgress, currentProgress, isIndeterminate);

            return builder.Build();
        }
    }
}
