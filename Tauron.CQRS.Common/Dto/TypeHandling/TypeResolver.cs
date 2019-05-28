using System;
using Newtonsoft.Json;
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
            serializer.Serialize(writer, serializer);

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.None:
                        break;
                    case JsonToken.StartObject:
                        break;
                    case JsonToken.StartArray:
                        break;
                    case JsonToken.StartConstructor:
                        break;
                    case JsonToken.PropertyName:
                        break;
                    case JsonToken.Comment:
                        break;
                    case JsonToken.Raw:
                        break;
                    case JsonToken.Integer:
                        break;
                    case JsonToken.Float:
                        break;
                    case JsonToken.String:
                        break;
                    case JsonToken.Boolean:
                        break;
                    case JsonToken.Null:
                        break;
                    case JsonToken.Undefined:
                        break;
                    case JsonToken.EndObject:
                        break;
                    case JsonToken.EndArray:
                        break;
                    case JsonToken.EndConstructor:
                        break;
                    case JsonToken.Date:
                        break;
                    case JsonToken.Bytes:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}