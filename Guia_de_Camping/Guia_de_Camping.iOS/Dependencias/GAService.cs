﻿using Foundation;
//using Google.Analytics;
using Keer.Dependencias;
using Xamarin.Forms;

[assembly: Dependency(typeof(Keer.iOS.Dependencias.GAService))]
namespace Keer.iOS.Dependencias
{
    public class GAService : IGAService
    {
        public string TrackingId = "UA-49846110-44";
        public string TrackerName = "Guia de Camping";
        //public ITracker Tracker;
        const string AllowTrackingKey = "AllowTracking";

        #region Instantition...
        private static GAService thisRef;
        public GAService()
        {
            // no code req'd
        }

        public static GAService GetGASInstance()
        {
            if (thisRef == null)
                // it's ok, we can call this constructor
                thisRef = new GAService();
            return thisRef;
        }
        #endregion

        public void Initialize_NativeGAS()
        {
            var optionsDict = NSDictionary.FromObjectAndKey(new NSString("NO"), new NSString(AllowTrackingKey));
            NSUserDefaults.StandardUserDefaults.RegisterDefaults(optionsDict);

            //Gai.SharedInstance.OptOut = !NSUserDefaults.StandardUserDefaults.BoolForKey(AllowTrackingKey);

            //Gai.SharedInstance.DispatchInterval = 10;
            //Gai.SharedInstance.TrackUncaughtExceptions = true;
            //
            //Tracker = Gai.SharedInstance.GetTracker(TrackerName, TrackingId);
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