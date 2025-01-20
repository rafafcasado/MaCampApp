using System.Diagnostics;

namespace MaCamp.Utils
{
    public static class GoogleAnalytics
    {
        public static string TrackerName => "Aspbrasil";
        public static string TrackingId => Debugger.IsAttached ? "UA-49846110-44" : "UA-49846110-36";
    }
}