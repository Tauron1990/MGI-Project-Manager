namespace FileServer.Lib
{
    public sealed class FileResult
    {
        public FileResult(bool error, string message)
        {
            Error = error;
            Message = message;
        }

        public bool Error { get; }

        public string Message { get; }
    }
}