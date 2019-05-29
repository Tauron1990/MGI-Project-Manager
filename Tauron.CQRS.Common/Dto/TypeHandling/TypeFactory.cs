using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.Dto.Persistable;

namespace Tauron.CQRS.Common.Dto.TypeHandling
{
    public static class TypeFactory
    {
        public static object Create(string originaltype, string data)
        {
            Type type = Type.GetType(originaltype);

            if (type == null) return null;

            var obj = Activator.CreateInstance(type);

            switch (obj)
            {
                case IJsonFormatable formatable:
                    formatable.Read(JToken.Parse(data));
                    break;
                default:
                    JsonConvert.PopulateObject(data, obj);
                    break;
            }

            return obj;
        }

        public static string Serialize(object data) 
            => data is IJsonFormatable formatable ? formatable.Create().ToString() : JsonConvert.SerializeObject(data);
    }
}