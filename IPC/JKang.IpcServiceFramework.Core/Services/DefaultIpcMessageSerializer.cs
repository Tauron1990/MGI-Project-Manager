using System.Text;
using Newtonsoft.Json;

namespace JKang.IpcServiceFramework
{
    public class DefaultIpcMessageSerializer : IIpcMessageSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
                                                                   {
                                                                       TypeNameHandling = TypeNameHandling.None
                                                                   };

        public IpcRequest DeserializeRequest(byte[] binary) => Deserialize<IpcRequest>(binary);

        public IpcResponse DeserializeResponse(byte[] binary) => Deserialize<IpcResponse>(binary);

        public byte[] SerializeRequest(IpcRequest request) => Serialize(request);

        public byte[] SerializeResponse(IpcResponse response) => Serialize(response);

        private static T Deserialize<T>(byte[] binary)
        {
            var json = Encoding.UTF8.GetString(binary);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static byte[] Serialize(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Settings);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}