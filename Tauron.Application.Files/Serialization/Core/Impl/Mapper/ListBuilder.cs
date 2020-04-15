using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization.Core.Impl.Mapper
{
    internal class ListBuilder : IEnumerable
    {
        private readonly bool _isArray;
        private readonly Type _listType;
        private Type _elemenType;
        private IEnumerable _enumerable;

        private IList _list;

        public ListBuilder([CanBeNull] Type listType)
        {
            if (listType == null) return;

            _isArray = listType.IsArray;

            _listType = _isArray ? typeof(ArrayList) : listType;
        }

        [NotNull]
        public object[] Objects => _enumerable.Cast<object>().ToArray();

        [CanBeNull]
        public Type ElemenType
        {
            get
            {
                if (_elemenType != null)
                    return _elemenType;

                _elemenType = GetElementType();
                return _elemenType;
            }
        }

        public IEnumerator GetEnumerator() => _list.GetEnumerator();

        [CanBeNull]
        private Type GetElementType()
        {
            Type elementType;

            if (_isArray)
                elementType = _listType.GetElementType();
            else
            {
                if (!_listType.IsGenericType) return null;

                elementType = _listType.GenericTypeArguments[0];

                var checkList = typeof(ICollection<>).MakeGenericType(elementType);

                return checkList.IsAssignableFrom(_listType) ? elementType : null;
            }

            return elementType;
        }

        public void Begin([CanBeNull] object enumerable, bool readingMode)
        {
            if (readingMode)
            {
                try
                {
                    _enumerable = (IEnumerable) enumerable;
                    return;
                }
                catch (InvalidCastException e)
                {
                    throw new InvalidOperationException("Invalid Cast", e);
                }
            }

            _list = Activator.CreateInstance(_listType) as IList;

            if (_list == null)
                throw new InvalidOperationException("No IList Implemented");
        }

        public void Add([CanBeNull] object value) => _list.Add(value);

        [CanBeNull]
        public object End()
        {
            try
            {
                if (_enumerable != null) return null;

                object value;

                if (_isArray)
                {
                    var arr = Array.CreateInstance(Argument.CheckResult(_listType.GetElementType(), "Initialization Error"), _list.Count);
                    _list.CopyTo(arr, 0);
                    value = arr;
                }
                else
                {
                    value = _list;
                }

                return value;
            }
            finally
            {
                _list = null;
                _enumerable = null;
            }
        }

        [CanBeNull]
        public Exception VerifyError()
        {
            if (_listType == null) return new SerializerElementNullException("Unkowen List Type");

            return _listType.IsAssignableFrom(typeof(IList))
                ? new SerializerElementException("In Compatible MemerType! Must implement IList")
                : null;
        }
    }
}