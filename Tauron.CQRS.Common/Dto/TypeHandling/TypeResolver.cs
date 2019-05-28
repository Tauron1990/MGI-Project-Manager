using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tauron.CQRS.Common.Dto.TypeHandling.Impl;

namespace Tauron.CQRS.Common.Dto.TypeHandling
{
    public class TypeResolver : JsonConverter
    {
        private const string PropertyName = "Type-Info";

        public static readonly ITypeRegistry TypeRegistry = new TypeRegistry();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string name = TypeRegistry.GetName(value.GetType());
            if(string.IsNullOrWhiteSpace(name)) return;

            writer.WriteStartObject();

            writer.WritePropertyName(PropertyName);
            writer.WriteValue(name);

            writer.WritePropertyName("Data");
            serializer.Serialize(writer, value);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!reader.Read()) return existingValue;

            var typeInfo = JProperty.Load(reader);
            var targetType = TypeRegistry.Resolve(typeInfo.Value.Value<string>());

            if (!reader.Read()) return existingValue;

            object obj;
            if (targetType == typeof(JToken))
                obj = JToken.Load(reader);
            else
                obj = targetType == null ? existingValue : serializer.Deserialize(reader, targetType);

            reader.Read();
            return obj;
        }

        public override bool CanConvert(Type objectType) 
            => TypeRegistry.Contains(objectType);
    }
}