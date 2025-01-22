namespace MaCamp.Dependencias
{
    public interface IGAService
    {
        void Track_App_Page(string pageNameToTrack);
        void Track_App_Event(string GAEventCategory, string EventToTrack);
        void Track_App_Exception(string ExceptionMessageToTrack, bool isFatalException);
    }
}