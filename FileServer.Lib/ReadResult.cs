namespace FileServer.Lib
{
    public sealed class ReadResult
    {
        public ReadResult(byte[] data, bool successful, long remaining, long lenght)
        {
            Data = data;
            Successful = successful;
            Remaining = remaining;
            Lenght = lenght;
        }

        public byte[] Data { get; }

        public bool Successful { get; }

        public long Remaining { get; }

        public long Lenght { get; }
    }
}