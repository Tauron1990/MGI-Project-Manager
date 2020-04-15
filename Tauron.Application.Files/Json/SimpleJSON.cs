using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Tauron.Application.Files.Json
{
    [PublicAPI]
    public enum JsonNodeType
    {
        Array = 1,
        Object = 2,
        String = 3,
        Number = 4,
        NullValue = 5,
        Boolean = 6,
        None = 7,
        Custom = 0xFF
    }

    public enum JsonTextMode
    {
        Compact,
        Indent
    }

    public abstract partial class JsonNode
    {
        [ThreadStatic]
        private static StringBuilder? _escapeBuilder;
        
        internal static StringBuilder EscapeBuilder => _escapeBuilder ??= new StringBuilder();

        internal static string Escape(string aText)
        {
            var sb = EscapeBuilder;
            sb.Length = 0;
            if (sb.Capacity < aText.Length + aText.Length / 10)
                sb.Capacity = aText.Length + aText.Length / 10;
            foreach (var c in aText)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < ' ' || ForceAscii && c > 127)
                        {
                            ushort val = c;
                            sb.Append("\\u").Append(val.ToString("X4"));
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                }
            }

            var result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        private static JsonNode ParseElement(string token, bool quoted)
        {
            if (quoted)
                return token;
            var tmp = token.ToLower();
            switch (tmp)
            {
                case "false":
                case "true":
                    return tmp == "true";
                case "null":
                    return JsonNull.CreateOrGet();
            }

            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
                return val;
            return token;
        }

        public static JsonNode Parse(string aJson)
        {
            var stack = new Stack<JsonNode>();
            JsonNode? ctx = null;
            var i = 0;
            var token = new StringBuilder();
            var tokenName = "";
            var quoteMode = false;
            var tokenIsQuoted = false;
            while (i < aJson.Length)
            {
                switch (aJson[i])
                {
                    case '{':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }

                        stack.Push(new JsonObject());
                        ctx?.Add(tokenName, stack.Peek());
                        tokenName = "";
                        token.Length = 0;
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }

                        stack.Push(new JsonArray());
                        ctx?.Add(tokenName, stack.Peek());
                        tokenName = "";
                        token.Length = 0;
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }

                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (token.Length > 0 || tokenIsQuoted)
                            ctx?.Add(tokenName, ParseElement(token.ToString(), tokenIsQuoted));

                        tokenIsQuoted = false;
                        tokenName = "";
                        token.Length = 0;
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }

                        tokenName = token.ToString();
                        token.Length = 0;
                        tokenIsQuoted = false;
                        break;

                    case '"':
                        quoteMode ^= true;
                        tokenIsQuoted |= quoteMode;
                        break;

                    case ',':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }

                        if (token.Length > 0 || tokenIsQuoted)
                                ctx?.Add(tokenName, ParseElement(token.ToString(), tokenIsQuoted));

                        tokenName = "";
                        token.Length = 0;
                        tokenIsQuoted = false;
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (quoteMode)
                            token.Append(aJson[i]);
                        break;

                    case '\\':
                        ++i;
                        if (quoteMode)
                        {
                            var c = aJson[i];
                            switch (c)
                            {
                                case 't':
                                    token.Append('\t');
                                    break;
                                case 'r':
                                    token.Append('\r');
                                    break;
                                case 'n':
                                    token.Append('\n');
                                    break;
                                case 'b':
                                    token.Append('\b');
                                    break;
                                case 'f':
                                    token.Append('\f');
                                    break;
                                case 'u':
                                {
                                    var s = aJson.Substring(i + 1, 4);
                                    token.Append((char) int.Parse(
                                        s,
                                        NumberStyles.AllowHexSpecifier));
                                    i += 4;
                                    break;
                                }
                                default:
                                    token.Append(c);
                                    break;
                            }
                        }

                        break;

                    default:
                        token.Append(aJson[i]);
                        break;
                }

                ++i;
            }

            if (quoteMode) throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            return ctx ?? ParseElement(token.ToString(), tokenIsQuoted);
        }

        #region Enumerators

        [PublicAPI]
        public struct Enumerator
        {
            private enum Type
            {
                None,
                Array,
                Object
            }

            private readonly Type _type;
            private Dictionary<string, JsonNode>.Enumerator _object;
            private List<JsonNode>.Enumerator _array;
            public bool IsValid => _type != Type.None;

            public Enumerator(List<JsonNode>.Enumerator aArrayEnum)
            {
                _type = Type.Array;
                _object = default;
                _array = aArrayEnum;
            }

            public Enumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum)
            {
                _type = Type.Object;
                _object = aDictEnum;
                _array = default;
            }

            public KeyValuePair<string, JsonNode> Current
            {
                get
                {
                    return _type switch
                    {
                        Type.Array => new KeyValuePair<string, JsonNode>(string.Empty, _array.Current),
                        Type.Object => _object.Current,
                        _ => throw new InvalidOperationException("No Element")
                    };
                }
            }

            public bool MoveNext()
            {
                return _type switch
                {
                    Type.Array => _array.MoveNext(),
                    Type.Object => _object.MoveNext(),
                    _ => false
                };
            }
        }

        [PublicAPI]
        public struct ValueEnumerator
        {
            private Enumerator _enumerator;

            public ValueEnumerator(List<JsonNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum))
            {
            }

            public ValueEnumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum))
            {
            }

            public ValueEnumerator(Enumerator aEnumerator) 
                => _enumerator = aEnumerator;

            public JsonNode? Current => _enumerator.Current.Value;

            public bool MoveNext() 
                => _enumerator.MoveNext();

            public ValueEnumerator GetEnumerator() 
                => this;
        }

        [PublicAPI]
        public struct KeyEnumerator
        {
            private Enumerator _enumerator;

            public KeyEnumerator(List<JsonNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum))
            {
            }

            public KeyEnumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum))
            {
            }

            public KeyEnumerator(Enumerator aEnumerator)
            {
                _enumerator = aEnumerator;
            }

            public JsonNode Current => _enumerator.Current.Key;

            public bool MoveNext() 
                => _enumerator.MoveNext();

            public KeyEnumerator GetEnumerator()
            {
                return this;
            }
        }

        [PublicAPI]
        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JsonNode>>, IEnumerable<KeyValuePair<string, JsonNode>>
        {
            private Enumerator _enumerator;
            private JsonNode? _node;

            internal LinqEnumerator(JsonNode? aNode)
            {
                _node = aNode;
                if (aNode != null)
                    _enumerator = aNode.GetEnumerator();
            }

            public IEnumerator<KeyValuePair<string, JsonNode>> GetEnumerator() 
                => new LinqEnumerator(_node);

            IEnumerator IEnumerable.GetEnumerator() 
                => new LinqEnumerator(_node);

            public KeyValuePair<string, JsonNode> Current => _enumerator.Current;
            object IEnumerator.Current => _enumerator.Current;

            public bool MoveNext() 
                => _enumerator.MoveNext();

            public void Dispose()
            {
                _node = null;
                _enumerator = new Enumerator();
            }

            public void Reset()
            {
                if (_node != null)
                    _enumerator = _node.GetEnumerator();
            }
        }

        #endregion Enumerators

        #region common interface

        public static bool ForceAscii { get; set; }
        public static bool LongAsString { get; set; }

        public abstract JsonNodeType Tag { get; }

        public abstract JsonNode? this[int aIndex] { get; set; }

        public abstract JsonNode? this[string aKey] { get; set; }

        public abstract string Value { get; set; }

        public virtual int Count => 0;

        public virtual bool IsNumber => false;
        public virtual bool IsString => false;
        public virtual bool IsBoolean => false;
        public virtual bool IsNull => false;
        public virtual bool IsArray => false;
        public virtual bool IsObject => false;

        public abstract bool Inline { get; set; }

        public virtual void Add(string aKey, JsonNode? aItem)
        {
        }

        public virtual void Add(JsonNode? aItem) => Add("", aItem);

        public virtual JsonNode? Remove(string aKey) => null;

        public virtual JsonNode? Remove(int aIndex) => null;

        public virtual JsonNode? Remove(JsonNode aNode) => aNode;

        public virtual IEnumerable<JsonNode> Children
        {
            get { yield break; }
        }

        public IEnumerable<JsonNode> DeepChildren => Children.SelectMany(jsonNode => jsonNode.DeepChildren);

        public override string ToString()
        {
            var sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, 0, JsonTextMode.Compact);
            return sb.ToString();
        }

        public string ToString(int aIndent)
        {
            var sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, aIndent, JsonTextMode.Indent);
            return sb.ToString();
        }

        internal abstract void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode);

        public abstract Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JsonNode>> Linq => new LinqEnumerator(this);
        public KeyEnumerator Keys => new KeyEnumerator(GetEnumerator());
        public ValueEnumerator Values => new ValueEnumerator(GetEnumerator());

        #endregion common interface

        #region typecasting properties

        public virtual double AsDouble
        {
            get => double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0.0;
            set => Value = value.ToString(CultureInfo.InvariantCulture);
        }

        public virtual int AsInt
        {
            get => (int) AsDouble;
            set => AsDouble = value;
        }

        public virtual float AsFloat
        {
            get => (float) AsDouble;
            set => AsDouble = value;
        }

        public virtual bool AsBool
        {
            get
            {
                if (bool.TryParse(Value, out var v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set => Value = value ? "true" : "false";
        }

        public virtual long AsLong
        {
            get => long.TryParse(Value, out var val) ? val : 0L;
            set => Value = value.ToString();
        }

        public virtual JsonArray? AsArray => this as JsonArray;

        public virtual JsonObject? AsObject => this as JsonObject;

        #endregion typecasting properties

        #region operators

        public static implicit operator JsonNode(string s) 
            => new JsonString(s);

        public static implicit operator string?(JsonNode d) 
            => d == null ? null : d.Value;

        public static implicit operator JsonNode(double n)
        {
            return new JsonNumber(n);
        }

        public static implicit operator double(JsonNode d)
        {
            return d == null ? 0 : d.AsDouble;
        }

        public static implicit operator JsonNode(float n)
        {
            return new JsonNumber(n);
        }

        public static implicit operator float(JsonNode d)
        {
            return d == null ? 0 : d.AsFloat;
        }

        public static implicit operator JsonNode(int n)
        {
            return new JsonNumber(n);
        }

        public static implicit operator int(JsonNode d)
        {
            return d == null ? 0 : d.AsInt;
        }

        public static implicit operator JsonNode(long n)
        {
            if (LongAsString)
                return new JsonString(n.ToString());
            return new JsonNumber(n);
        }

        public static implicit operator long(JsonNode d)
        {
            return d == null ? 0L : d.AsLong;
        }

        public static implicit operator JsonNode(bool b)
        {
            return new JsonBool(b);
        }

        public static implicit operator bool(JsonNode d)
        {
            return d != null && d.AsBool;
        }

        public static implicit operator JsonNode(KeyValuePair<string, JsonNode> aKeyValue)
        {
            return aKeyValue.Value;
        }

        public static bool operator ==(JsonNode? a, object? b)
        {
            if (ReferenceEquals(a, b))
                return true;
            var aIsNull = a is JsonNull || ReferenceEquals(a, null) || a is JsonLazyCreator;
            var bIsNull = b is JsonNull || ReferenceEquals(b, null) || b is JsonLazyCreator;
            if (aIsNull && bIsNull)
                return true;
            return !aIsNull && a!.Equals(b!);
        }

        public static bool operator !=(JsonNode? a, object? b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators
    }
    // End of JSONNode

    public partial class JsonArray : JsonNode
    {
        private bool _inline;
        private readonly List<JsonNode> _list = new List<JsonNode>();

        public override bool Inline
        {
            get => _inline;
            set => _inline = value;
        }

        public override JsonNodeType Tag => JsonNodeType.Array;
        public override bool IsArray => true;

        public override JsonNode? this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= _list.Count)
                    return new JsonLazyCreator(this);
                return _list[aIndex];
            }
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= _list.Count)
                    _list.Add(value);
                else
                    _list[aIndex] = value;
            }
        }

        public override JsonNode? this[string aKey]
        {
            get => new JsonLazyCreator(this);
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                _list.Add(value);
            }
        }

        public override string Value
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Count => _list.Count;

        public override IEnumerable<JsonNode> Children
        {
            get
            {
                foreach (var children in _list)
                    yield return children;
            }
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator(_list.GetEnumerator());
        }

        public override void Add(string aKey, JsonNode? aItem)
        {
            if (aItem == null)
                aItem = JsonNull.CreateOrGet();
            _list.Add(aItem);
        }

        public override JsonNode? Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= _list.Count)
                return null;
            var tmp = _list[aIndex];
            _list.RemoveAt(aIndex);
            return tmp;
        }

        public override JsonNode Remove(JsonNode aNode)
        {
            _list.Remove(aNode);
            return aNode;
        }


        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append('[');
            var count = _list.Count;
            if (_inline)
                aMode = JsonTextMode.Compact;
            for (var i = 0; i < count; i++)
            {
                if (i > 0)
                    aSb.Append(',');
                if (aMode == JsonTextMode.Indent)
                    aSb.AppendLine();

                if (aMode == JsonTextMode.Indent)
                    aSb.Append(' ', aIndent + aIndentInc);
                _list[i].WriteToStringBuilder(aSb, aIndent + aIndentInc, aIndentInc, aMode);
            }

            if (aMode == JsonTextMode.Indent)
                aSb.AppendLine().Append(' ', aIndent);
            aSb.Append(']');
        }
    }
    // End of JSONArray

    public partial class JsonObject : JsonNode
    {
        private readonly Dictionary<string, JsonNode> _dict = new Dictionary<string, JsonNode>();
        private bool _inline;

        public override bool Inline
        {
            get => _inline;
            set => _inline = value;
        }

        public override JsonNodeType Tag => JsonNodeType.Object;
        public override bool IsObject => true;


        public override JsonNode? this[string aKey]
        {
            get => _dict.ContainsKey(aKey) ? _dict[aKey] : new JsonLazyCreator(this, aKey);
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                if (_dict.ContainsKey(aKey))
                    _dict[aKey] = value;
                else
                    _dict.Add(aKey, value);
            }
        }

        public override string Value
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override JsonNode? this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= _dict.Count)
                    return null;
                return _dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= _dict.Count)
                    return;
                var key = _dict.ElementAt(aIndex).Key;
                _dict[key] = value;
            }
        }

        public override int Count => _dict.Count;

        public override IEnumerable<JsonNode> Children 
            => _dict.Select(pair => pair.Value);

        public override Enumerator GetEnumerator() 
            => new Enumerator(_dict.GetEnumerator());

        public override void Add(string aKey, JsonNode? aItem)
        {
            if (aItem == null)
                aItem = JsonNull.CreateOrGet();

            if (!string.IsNullOrEmpty(aKey))
            {
                if (_dict.ContainsKey(aKey))
                    _dict[aKey] = aItem;
                else
                    _dict.Add(aKey, aItem);
            }
            else
            {
                _dict.Add(Guid.NewGuid().ToString(), aItem);
            }
        }

        public override JsonNode? Remove(string aKey)
        {
            if (!_dict.ContainsKey(aKey))
                return null;
            var tmp = _dict[aKey];
            _dict.Remove(aKey);
            return tmp;
        }

        public override JsonNode? Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= _dict.Count)
                return null;
            var item = _dict.ElementAt(aIndex);
            _dict.Remove(item.Key);
            return item.Value;
        }

        public override JsonNode? Remove(JsonNode aNode)
        {
            try
            {
                var item = _dict.First(k => k.Value == aNode);
                _dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append('{');
            var first = true;
            if (_inline)
                aMode = JsonTextMode.Compact;
            foreach (var k in _dict)
            {
                if (!first)
                    aSb.Append(',');
                first = false;
                if (aMode == JsonTextMode.Indent)
                    aSb.AppendLine();
                if (aMode == JsonTextMode.Indent)
                    aSb.Append(' ', aIndent + aIndentInc);
                aSb.Append('\"').Append(Escape(k.Key)).Append('\"');
                if (aMode == JsonTextMode.Compact)
                    aSb.Append(':');
                else
                    aSb.Append(" : ");
                k.Value.WriteToStringBuilder(aSb, aIndent + aIndentInc, aIndentInc, aMode);
            }

            if (aMode == JsonTextMode.Indent)
                aSb.AppendLine().Append(' ', aIndent);
            aSb.Append('}');
        }
    }
    // End of JSONObject

    public partial class JsonString : JsonNode
    {
        private string _data;

        public JsonString(string aData)
        {
            _data = aData;
        }

        public override JsonNodeType Tag => JsonNodeType.String;

        public override JsonNode? this[int aIndex]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override JsonNode? this[string aKey]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override bool IsString => true;

        public override bool Inline
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }


        public override string Value
        {
            get => _data;
            set => _data = value;
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator();
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append('\"').Append(Escape(_data)).Append('\"');
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;
            if (obj is string s)
                return _data == s;
            var s2 = obj as JsonString;
            if (s2 != null)
                return _data == s2._data;
            return false;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }
    }
    // End of JSONString

    [PublicAPI]
    public partial class JsonNumber : JsonNode
    {
        private double _data;

        public JsonNumber(double aData)
        {
            _data = aData;
        }

        public JsonNumber(string aData)
        {
            Value = aData;
        }

        public override JsonNodeType Tag => JsonNodeType.Number;

        public override JsonNode? this[int aIndex]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override JsonNode? this[string aKey]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override bool IsNumber => true;

        public override bool Inline
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public sealed override string Value
        {
            get => _data.ToString(CultureInfo.InvariantCulture);
            set
            {
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
                    _data = v;
            }
        }

        public override double AsDouble
        {
            get => _data;
            set => _data = value;
        }

        public override long AsLong
        {
            get => (long) _data;
            set => _data = value;
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator();
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append(Value);
        }

        private static bool IsNumeric(object value)
        {
            return value is int || value is uint
                                || value is float || value is double
                                || value is decimal
                                || value is long || value is ulong
                                || value is short || value is ushort
                                || value is sbyte || value is byte;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (base.Equals(obj))
                return true;
            var s2 = obj as JsonNumber;
            if (s2 != null)
                return Math.Abs(_data - s2._data) < 0;
            if (IsNumeric(obj))
                return Math.Abs(Convert.ToDouble(obj) - _data) < 0;
            return false;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }
    }
    // End of JSONNumber
    [PublicAPI]
    public partial class JsonBool : JsonNode
    {
        private bool _data;

        public JsonBool(bool aData)
        {
            _data = aData;
        }

        public JsonBool(string aData)
        {
            Value = aData;
        }

        public override JsonNodeType Tag => JsonNodeType.Boolean;

        public override JsonNode? this[int aIndex]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override JsonNode? this[string aKey]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override bool IsBoolean => true;

        public override bool Inline
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public sealed override string Value
        {
            get => _data.ToString();
            set
            {
                if (bool.TryParse(value, out var v))
                    _data = v;
            }
        }

        public override bool AsBool
        {
            get => _data;
            set => _data = value;
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator();
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append(_data ? "true" : "false");
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case bool b:
                    return _data == b;
                default:
                    return false;
            }
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }
    }
    // End of JSONBool

    public partial class JsonNull : JsonNode
    {
        private static readonly JsonNull StaticInstance = new JsonNull();
        public static bool ReuseSameInstance = true;

        private JsonNull()
        {
        }

        public override JsonNodeType Tag => JsonNodeType.NullValue;

        public override JsonNode? this[int aIndex]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override JsonNode? this[string aKey]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override bool IsNull => true;

        public override bool Inline
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override string Value
        {
            get => "null";
            set { }
        }

        public override bool AsBool
        {
            get => false;
            set { }
        }

        public static JsonNull CreateOrGet()
        {
            return ReuseSameInstance ? StaticInstance : new JsonNull();
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            return obj is JsonNull;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append("null");
        }
    }
    // End of JSONNull

    internal partial class JsonLazyCreator : JsonNode
    {
        private readonly string? _key;
        private JsonNode? _node;

        public JsonLazyCreator(JsonNode aNode)
        {
            _node = aNode;
            _key = null;
        }

        public JsonLazyCreator(JsonNode aNode, string aKey)
        {
            _node = aNode;
            _key = aKey;
        }

        public override JsonNodeType Tag => JsonNodeType.None;

        public override JsonNode? this[int aIndex]
        {
            get => new JsonLazyCreator(this);
            set => Set(new JsonArray()).Add(value);
        }

        public override JsonNode? this[string aKey]
        {
            get => new JsonLazyCreator(this, aKey);
            set => Set(new JsonObject()).Add(aKey, value);
        }

        public override string Value
        {
            get => string.Empty;
            set => throw new NotSupportedException();
        }

        public override int AsInt
        {
            get
            {
                Set(new JsonNumber(0));
                return 0;
            }
            set => Set(new JsonNumber(value));
        }

        public override float AsFloat
        {
            get
            {
                Set(new JsonNumber(0.0f));
                return 0.0f;
            }
            set => Set(new JsonNumber(value));
        }

        public override double AsDouble
        {
            get
            {
                Set(new JsonNumber(0.0));
                return 0.0;
            }
            set => Set(new JsonNumber(value));
        }

        public override long AsLong
        {
            get
            {
                if (LongAsString)
                    Set(new JsonString("0"));
                else
                    Set(new JsonNumber(0.0));
                return 0L;
            }
            set
            {
                if (LongAsString)
                    Set(new JsonString(value.ToString()));
                else
                    Set(new JsonNumber(value));
            }
        }

        public override bool AsBool
        {
            get
            {
                Set(new JsonBool(false));
                return false;
            }
            set => Set(new JsonBool(value));
        }

        public override JsonArray AsArray => Set(new JsonArray());

        public override JsonObject AsObject => Set(new JsonObject());

        public override bool Inline
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override Enumerator GetEnumerator()
        {
            return new Enumerator();
        }

        private T Set<T>(T aVal) where T : JsonNode
        {
            if (_key == null)
                _node?.Add(aVal);
            else
                _node?.Add(_key, aVal);
            _node = null; // Be GC friendly.
            return aVal;
        }

        public override void Add(JsonNode? aItem)
        {
            Set(new JsonArray()).Add(aItem);
        }

        public override void Add(string aKey, JsonNode? aItem)
        {
            Set(new JsonObject()).Add(aKey, aItem);
        }

        public static bool operator ==(JsonLazyCreator a, object b)
        {
            return b == null || ReferenceEquals(a, b);
        }

        public static bool operator !=(JsonLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj == null || ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append("null");
        }
    }
    // End of JSONLazyCreator

    [PublicAPI]
    public static class Json
    {
        public static JsonNode Parse(string aJson)
        {
            return JsonNode.Parse(aJson);
        }
    }
}