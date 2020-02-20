namespace FileServer.Lib
{
    public sealed class FileResult
    {
        public bool Error { get; }

        public string Message { get; }

        public FileResult(bool error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}