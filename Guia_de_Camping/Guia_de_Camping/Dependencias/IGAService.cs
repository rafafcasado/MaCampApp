namespace Keer.Dependencias
{
    public interface IGAService
    {
        void Track_App_Page(string PageNameToTrack);

        void Track_App_Event(string GAEventCategory, string EventToTrack);

        void Track_App_Exception(string ExceptionMessageToTrack, bool isFatalException);
    }

    public struct GAEventCategory
    {
        public static string Category1 { get { return "Categoria 1"; } set { } }
        public static string Category2 { get { return "Categoria 2"; } set { } }
        public static string Category3 { get { return "Categoria 3"; } set { } }
    };
}
