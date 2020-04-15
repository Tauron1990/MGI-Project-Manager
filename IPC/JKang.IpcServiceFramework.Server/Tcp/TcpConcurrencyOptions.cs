namespace JKang.IpcServiceFramework.Tcp
{
    public class TcpConcurrencyOptions
    {
        public int MaximumConcurrentCalls;

        public TcpConcurrencyOptions(int maximumConcurrentCalls) => MaximumConcurrentCalls = maximumConcurrentCalls;
    }
}