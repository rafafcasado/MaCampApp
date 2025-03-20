namespace MaCamp.Utils
{
    public class NotificationData
    {
        public string Title { get; set; }
        public string? Message { get; set; }
        public int? ProgressValue { get; set; }

        public NotificationData()
        {
            Title = string.Empty;
        }
    }
}
