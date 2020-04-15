using Newtonsoft.Json;

namespace JKang.IpcServiceFramework
{
    public class IpcResponse
    {
        [JsonConstructor]
        private IpcResponse(bool succeed, object? data, string? failure)
        {
            Succeed = succeed;
            Data = data;
            Failure = failure;
        }

        public bool Succeed { get; }
        public object? Data { get; }
        public string? Failure { get; }

        public static IpcResponse Fail(string failure) => new IpcResponse(false, null, failure);

        public static IpcResponse Success(object data) => new IpcResponse(true, data, null);
    }
}