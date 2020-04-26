namespace Akka.Copy.Messages
{
    public sealed class SingleCopyUpdate
    {
        public bool Skiped { get; }

        public long Size { get; }


        public SingleCopyUpdate(bool skiped, long size)
        {
            Skiped = skiped;
            Size = size;
        }
    }
}