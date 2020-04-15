using System;
using System.Collections.Generic;
using System.Linq;
using Ionic.Zip;
using JetBrains.Annotations;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public sealed class InternalZipDirectory
    {
        public static readonly char[] PathSplit = {'/'};

        private InternalZipDirectory(string name)
        {
            Name = name;
            Files = new List<ZipEntry>();
            Directorys = new List<InternalZipDirectory>();
        }

        public string Name { get; }

        public ZipEntry? ZipEntry { get; private set; }

        public List<ZipEntry> Files { get; }

        public List<InternalZipDirectory> Directorys { get; }

        public static InternalZipDirectory ReadZipDirectory(ZipFile? file)
        {
            var directory = new InternalZipDirectory(string.Empty);
            if (file == null) return directory;

            foreach (var entry in file) Add(directory, entry);

            return directory;
        }

        private static void Add(InternalZipDirectory directory, ZipEntry entry)
        {
            var parts = entry.FileName.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries);

            var mainDic = directory;

            for (var i = 0; i < parts.Length; i++)
                if (i == parts.Length - 1)
                {
                    if (entry.IsDirectory)
                    {
                        mainDic = mainDic.GetOrAdd(parts[i]);
                        mainDic.ZipEntry = entry;
                    }
                    else
                    {
                        mainDic.AddFile(entry);
                    }
                }
                else
                {
                    mainDic = mainDic.GetOrAdd(parts[i]);
                }
        }

        public static string GetFileName(ZipEntry entry)
        {
            return Argument.NotNull(entry, nameof(entry)).FileName.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        internal InternalZipDirectory GetOrAdd(string name)
        {
            var dic = Directorys.FirstOrDefault(d => d.Name == name);
            if (dic != null) return dic;
            dic = new InternalZipDirectory(name);
            Directorys.Add(dic);
            return dic;
        }

        private void AddFile([NotNull] ZipEntry entry)
        {
            Files.Add(entry);
        }
    }
}