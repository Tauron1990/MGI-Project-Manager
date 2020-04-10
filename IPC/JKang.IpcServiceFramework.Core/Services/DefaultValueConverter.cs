using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JKang.IpcServiceFramework.Services
{
    public class DefaultValueConverter : IValueConverter
    {
        public bool TryConvert(object origValue, Type destType, out object destValue)
        {
            if (origValue == null)
            {
                destValue = null;
                return destType.IsClass || Nullable.GetUnderlyingType(destType) != null;
            }

            if (destType.IsInstanceOfType(origValue))
            {
                // copy value directly if it can be assigned to destType
                destValue = origValue;
                return true;
            }

            if (destType.IsEnum)
            {
                if (origValue is string str)
                {
                    try
                    {
                        destValue = Enum.Parse(destType, str, true);
                        return true;
                    }
                    catch (SystemException)
                    {
                    }
                }
                else
                {
                    try
                    {
                        destValue = Enum.ToObject(destType, origValue);
                        return true;
                    }
                    catch(SystemException)
                    {
                    }
                }
            }

            switch (origValue)
            {
                case string str2 when destType == typeof(Guid) && Guid.TryParse(str2, out var result):
                    destValue = result;
                    return true;
                case JObject jObj:
                {
                    if (destType.IsInterface || destType.IsClass && destType.IsAbstract)
                    {
                        var kT = (KnowType) Attribute.GetCustomAttributes(destType).First(x => x.GetType() == typeof(KnowType));
                        if (kT != null)
                        {
                            destValue = jObj.ToObject(kT.Type);
                            return true;
                        }
                    }

                    destValue = jObj.ToObject(destType);
                    // TODO: handle error
                    return true;
                }
                case JArray jArray:
                    destValue = jArray.ToObject(destType);
                    return true;
            }

            try
            {
                destValue = Convert.ChangeType(origValue, destType);
                return true;
            }
            catch (SystemException)
            {
            }

            try
            {
                destValue = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(origValue), destType);
                return true;
            }
            catch (SystemException)
            {
            }

            destValue = null;
            return false;
        }
    }
}