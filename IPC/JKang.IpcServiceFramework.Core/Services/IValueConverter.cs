using System;
using System.Diagnostics.CodeAnalysis;

namespace JKang.IpcServiceFramework.Services
{
    public interface IValueConverter
    {
        bool TryConvert(object origValue, Type destType, out object? destValue);
    }
}