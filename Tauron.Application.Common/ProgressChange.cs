namespace Tauron
{
    /// <summary>
    /// A delegate for reporting binary progress
    /// </summary>
    /// <param name="bytesRead">The amount of bytes already read</param>
    /// <param name="totalBytesToRead">The amount of total bytes to read. Can be -1 if unknown.</param>
    public delegate void ProgressChange(long bytesRead, long totalBytesToRead);
}