namespace Aspbrasil.AppSettings
{
    public static class GoogleAnalytics
    {
        public const string TRACKER_NAME = "Aspbrasil";

#if DEBUG
        public const string TRACKING_ID = "UA-49846110-44";
#else
        public const string TRACKING_ID = "UA-49846110-36";
#endif
    }
}