namespace MaCamp.Dependencias
{
    public interface IGAService
    {
        void Track_App_Page(string pageNameToTrack);
        void Track_App_Event(string GAEventCategory, string EventToTrack);
        void Track_App_Exception(string ExceptionMessageToTrack, bool isFatalException);
    }

    public struct GAEventCategory
    {
        public static string Category1 => "Categoria 1";
        public static string Category2 => "Categoria 2";
        public static string Category3 => "Categoria 3";
    };
}