using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Code.Configuration.Converter;
using Akka.Code.Configuration.Elements;
using Akka.Code.Configuration.Serialization;
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

            public string? Convert() => Converter.ToElementValue(Value);
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

        protected TType GetMergeElement<TType>()
            where TType : ConfigurationElement, new()
        {
            var element = _toMerge.OfType<TType>().FirstOrDefault();
            if (element != null) return element;

            element = new TType();
            _toMerge.Add(element);
            return element;

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
                var content = value.Convert();
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
            throw new NotImplementedException();
        }

        void IBinarySerializable.Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}