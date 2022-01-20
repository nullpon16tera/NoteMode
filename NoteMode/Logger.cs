using IPALogger = IPA.Logging.Logger;

namespace NoteMode
{
    internal static class Logger
    {
        internal static IPALogger log => Plugin.Log;
    }
}