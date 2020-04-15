using System.IO;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles;

namespace Tauron.Application.Files
{
    [PublicAPI]
    public static class IoExtensions
    {
        public static void WriteAllBytes(this IFile file, byte[] bytes)
        {
            using var write = file.Create();
            write.Write(bytes, 0, bytes.Length);
        }

        public static byte[] ReadAllBytes(this IFile file)
        {
            if (!file.Exist) return new byte[0];

            using var stream = file.Open(FileAccess.Read);
            byte[] arr = new byte[stream.Length];
            stream.Read(arr, 0, arr.Length);

            return arr;
        }
    }
}