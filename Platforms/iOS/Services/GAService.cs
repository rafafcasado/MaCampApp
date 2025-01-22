using Foundation;
using MaCamp.Dependencias;
using MaCamp.Utils;

namespace MaCamp.Platforms.iOS.Services
{
    public class GAService : IGAService
    {
        //public ITracker Tracker { get; set; }
        public static GAService Instance => AppExtensions.GetInstance<GAService>();

        public void Initialize_NativeGAS()
        {
            var optionsDict = NSDictionary.FromObjectAndKey(new NSString("NO"), new NSString(AppConstants.GAService_AllowTrackingKey));

            NSUserDefaults.StandardUserDefaults.RegisterDefaults(optionsDict);

            //Gai.SharedInstance.OptOut = !NSUserDefaults.StandardUserDefaults.BoolForKey(AppConstants.GAService_AllowTrackingKey);

            //Gai.SharedInstance.DispatchInterval = 10;
            //Gai.SharedInstance.TrackUncaughtExceptions = true;
            //
            //Tracker = Gai.SharedInstance.GetTracker(AppConstants.GAService_TrackerName, AppConstants.GAService_TrackingId);
        }

        public void Track_App_Page(string PageNameToTrack)
        {
            //Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, PageNameToTrack);
            //Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
        }

        public void Track_App_Event(string GAEventCategory, string EventToTrack)
        {
            //Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateEvent(GAEventCategory, EventToTrack, "AppEvent", null).Build());
            //Gai.SharedInstance.Dispatch(); // Manually dispatch the event immediately
        }

        public void Track_App_Exception(string ExceptionMessageToTrack, bool isFatalException)
        {
            //Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateException(ExceptionMessageToTrack, isFatalException).Build());
        }
    }
}