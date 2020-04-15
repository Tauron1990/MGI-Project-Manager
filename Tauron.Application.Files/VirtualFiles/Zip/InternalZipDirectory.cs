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

        private InternalZipDirectory([NotNull] string name)
        {
            Name = name;
            Files = new List<ZipEntry>();
            Directorys = new List<InternalZipDirectory>();
        }

        [NotNull]
        public string Name { get; }

        [CanBeNull]
        public ZipEntry ZipEntry { get; private set; }

        [NotNull]
        public List<ZipEntry> Files { get; }

        [NotNull]
        public List<InternalZipDirectory> Directorys { get; }

        [NotNull]
        public static InternalZipDirectory ReadZipDirectory([CanBeNull] ZipFile file)
        {
            var directory = new InternalZipDirectory(string.Empty);
            if (file == null) return directory;

            foreach (var entry in file)
            {
                Add(directory, entry);
            }

            return directory;
        }

        private static void Add([NotNull] InternalZipDirectory directory, [NotNull] ZipEntry entry)
        {
            var parts = entry.FileName.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries);

            var mainDic = directory;

            for (var i = 0; i < parts.Length; i++)
            {
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
        }

        [NotNull]
        public static string GetFileName([NotNull] ZipEntry entry) => Argument.NotNull(entry, nameof(entry)).FileName.Split(PathSplit, StringSplitOptions.RemoveEmptyEntries).Last();

        [NotNull]
        internal InternalZipDirectory GetOrAdd([NotNull] string name)
        {
            var dic = Directorys.FirstOrDefault(d => d.Name == name);
            if (dic != null) return dic;
            dic = new InternalZipDirectory(name);
            Directorys.Add(dic);
            return dic;
        }

        private void AddFile([NotNull] ZipEntry entry) => Files.Add(entry);
    }
}