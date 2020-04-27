using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Akka.Code.Configuration.Converter;
using Akka.Code.Configuration.Elements;
using Akka.Code.Configuration.Serialization;
using Akka.Util.Internal;
using JetBrains.Annotations;

namespace Akka.Code.Configuration
{
    [PublicAPI]
    public abstract class ConfigurationElement : IBinarySerializable
    {
        private sealed class DataValue
        {
            public object? Value { get; set; }

            public Type TargetType { get; }

            private ConverterBase Converter { get; }

            public DataValue(object? value, ConverterBase converter, Type targetType)
            {
                Value = value;
                TargetType = targetType;
                Converter = converter;
            }

            public DataValue(BinaryReader reader)
            {
                TargetType = Type.GetType(reader.ReadString());
                Converter = ConverterBase.Find(TargetType);
                if (reader.ReadBoolean())
                {
                    Value = null;
                    return;
                }

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch ((TypeCode)reader.ReadInt32())
                {
                    case TypeCode.Boolean:
                        Value = reader.ReadBoolean();
                        break;
                    case TypeCode.Byte:
                        Value = reader.ReadByte();
                        break;
                    case TypeCode.Char:
                        Value = reader.ReadChar();
                        break;
                    case TypeCode.DateTime:
                        Value = DateTime.FromBinary(reader.ReadInt64());
                        break;
                    case TypeCode.Decimal:
                        Value = reader.ReadDecimal();
                        break;
                    case TypeCode.Double:
                        Value = reader.ReadDouble();
                        break;
                    case TypeCode.Int16:
                        Value = reader.ReadInt16();
                        break;
                    case TypeCode.Int32:
                        Value = reader.ReadInt32();
                        break;
                    case TypeCode.Int64:
                        Value = reader.ReadInt64();
                        break;
                    case TypeCode.SByte:
                        Value = reader.ReadSByte();
                        break;
                    case TypeCode.Single:
                        Value = reader.ReadSingle();
                        break;
                    case TypeCode.String:
                        Value = reader.ReadString();
                        break;
                    case TypeCode.UInt16:
                        Value = reader.ReadUInt16();
                        break;
                    case TypeCode.UInt32:
                        Value = reader.ReadUInt32();
                        break;
                    case TypeCode.UInt64:
                        Value = reader.ReadUInt64();
                        break;
                    case TypeCode.Object:
                        if (TargetType == typeof(AkkaType)) 
                            Value = new AkkaType(reader.ReadString());
                        else if (TargetType.IsEnum)
                            Value = Enum.Parse(TargetType, reader.ReadString());
                        else if(typeof(ConfigurationElement).IsAssignableFrom(TargetType))
                        {
                            var ele = Activator.CreateInstance(Type.GetType(reader.ReadString()));
                            ((IBinarySerializable)ele).Read(reader);
                            Value = ele;
                        }
                        else
                        {
                            var converter = TypeDescriptor.GetConverter(TargetType);
                            if(converter == null)
                                throw new FormatException("Binary Data not Supportet Format");

                            Value = converter.ConvertFromString(reader.ReadString());
                        }
                        break;
                    default:
                        throw new FormatException("Binary Data not Supportet Format");
                }
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(TargetType.AssemblyQualifiedName);
                if (Value == null)
                {
                    writer.Write(true);
                    return;
                }

                writer.Write(false);
                writer.Write((int)Convert.GetTypeCode(Value));

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (Convert.GetTypeCode(Value))
                {
                    case TypeCode.Boolean:
                        writer.Write((bool)Value);
                        break;
                    case TypeCode.Byte:
                        writer.Write((byte)Value);
                        break;
                    case TypeCode.Char:
                        writer.Write((char)Value);
                        break;
                    case TypeCode.DateTime:
                        writer.Write(((DateTime)Value).ToBinary());
                        break;
                    case TypeCode.Decimal:
                        writer.Write((decimal)Value);
                        break;
                    case TypeCode.Double:
                        writer.Write((double)Value);
                        break;
                    case TypeCode.Int16:
                        writer.Write((short)Value);
                        break;
                    case TypeCode.Int32:
                        writer.Write((int)Value);
                        break;
                    case TypeCode.Int64:
                        writer.Write((long)Value);
                        break;
                    case TypeCode.SByte:
                        writer.Write((sbyte)Value);
                        break;
                    case TypeCode.Single:
                        writer.Write((float)Value);
                        break;
                    case TypeCode.String:
                        writer.Write((string)Value);
                        break;
                    case TypeCode.UInt16:
                        writer.Write((ushort)Value);
                        break;
                    case TypeCode.UInt32:
                        writer.Write((uint)Value);
                        break;
                    case TypeCode.UInt64:
                        writer.Write((ulong)Value);
                        break;
                    case TypeCode.Object:
                        switch (Value)
                        {
                            case Enum _:
                                writer.Write(Value.ToString());
                                break;
                            case AkkaType akkaType:
                                writer.Write(akkaType.Type);
                                break;
                            case ConfigurationElement ele:
                                writer.Write(ele.GetType().AssemblyQualifiedName);
                                ((IBinarySerializable)ele).Write(writer);
                                break;
                            default:
                                var converter = TypeDescriptor.GetConverter(TargetType);
                                if(converter == null)
                                    throw new NotSupportedException("Object type Not Supportet");
                                writer.Write(converter.ConvertToString(Value));
                                break;
                        }
                        break;
                    default:
                        throw new NotSupportedException("Object type Not Supportet");
                }
            }

            public string? ConvertToString() => Converter.ToElementValue(Value);
        }

        private Dictionary<string, ConfigurationElement> _toAdd = new Dictionary<string, ConfigurationElement>();
        private List<ConfigurationElement> _toMerge = new List<ConfigurationElement>();
        private Lazy<Dictionary<string, DataValue>> _data = new Lazy<Dictionary<string, DataValue>>();

        protected IEnumerable<KeyValuePair<string, ConfigurationElement>> ToAddElements => _toAdd.Where(e => true);

        protected TType Set<TType>(TType value, string name)
        {
            var targetType = typeof(TType);
            var data = _data.Value;

            if (data.TryGetValue(name, out var dataValue) && dataValue.TargetType == targetType) dataValue.Value = value;
            else data[name] = new DataValue(value, ConverterBase.Find(targetType), targetType);

            return value;
        }

        protected TType Get<TType>(string name)
        {
            if (_data.IsValueCreated && _data.Value.TryGetValue(name, out var value) && value.Value is TType typedType)
                return typedType;
            return default!;
        }

        protected bool Contains(string name) 
            => _data.IsValueCreated && _data.Value.ContainsKey(name);

        protected TType GetOrAdd<TType>(string name, Func<TType> fac)
            => Contains(name) ? Get<TType>(name) : Set(fac(), name);

        protected TType GetAddElement<TType>(string name)
            where TType : ConfigurationElement, new()
        {
            if (_toAdd.TryGetValue(name, out var ele))
                return (TType) ele;

            var element = new TType();
            _toAdd[name] = element;

            return element;
        }

        protected TType? TryGetMergeElement<TType>()
            where TType : ConfigurationElement =>
            _toMerge.OfType<TType>().FirstOrDefault();

        protected TType GetMergeElement<TType>()
            where TType : ConfigurationElement, new()
        {
            var element = _toMerge.OfType<TType>().FirstOrDefault();
            if (element != null) return element;

            element = new TType();
            _toMerge.Add(element);
            return element;

        }

        protected void ReplaceMerge<TType>(TType? target)
            where TType : ConfigurationElement
        {
            var element = _toMerge.OfType<TType>().FirstOrDefault();
            if (element != null) _toMerge.Remove(element);
            if(target == null) return;

            _toMerge.Add(target);
        }

        public virtual void Add(string name, ConfigurationElement element)
            => _toAdd[name] = element;

        public virtual void Merge(ConfigurationElement element) 
            => _toMerge.Add(element);

        public virtual Dictionary<string, object>? Construct()
        {
            if (_toAdd.Count == 0 && _toMerge.Count == 0 && !_data.IsValueCreated)
                return null;

            var target = new Dictionary<string, object>();

            foreach (var (key, value) in _data.Value)
            {
                var content = value.ConvertToString();
                if(content == null) continue;

                target[key] = content;
            }

            foreach (var (key, value) in _toMerge.SelectMany(d => d.Construct())) 
                target[key] = value;

            foreach (var (key, value) in _toAdd)
            {
                var content = value.Construct();
                if(content == null) continue;
                target[key] = content;
            }

            return target;
        }

        void IBinarySerializable.Write(BinaryWriter writer)
        {
            void Write(IBinarySerializable element)
            {
                writer.Write(element.GetType().AssemblyQualifiedName);
                element.Write(writer);
            }

            writer.Write(_toAdd.Count);
            _toAdd.ForEach(e =>
            {
                writer.Write(e.Key);
                Write(e.Value);
            });

            writer.Write(_toMerge.Count);
            _toMerge.ForEach(Write);

            if (_data.IsValueCreated)
            {
                writer.Write(true);
                writer.Write(_data.Value.Count);
                foreach (var (dataKey, dataValue) in _data.Value)
                {
                    writer.Write(dataKey);
                    dataValue.Write(writer);
                }
            }
            else
                writer.Write(false);
        }

        void IBinarySerializable.Read(BinaryReader reader)
        {
            ConfigurationElement Read()
            {
                var element = (ConfigurationElement) Activator.CreateInstance(Type.GetType(reader.ReadString()));
                ((IBinarySerializable)element).Read(reader);
                return element;
            }

            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++) 
                _toAdd[reader.ReadString()] = Read();

            count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
                _toMerge.Add(Read());

            if(!reader.ReadBoolean()) return;

            var data = _data.Value;
            count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
                data[reader.ReadString()] = new DataValue(reader);
        }
    }
}