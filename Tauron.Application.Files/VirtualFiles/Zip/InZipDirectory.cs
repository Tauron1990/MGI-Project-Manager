using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using JetBrains.Annotations;
using Tauron.Application.Files.VirtualFiles.Core;

namespace Tauron.Application.Files.VirtualFiles.Zip
{
    public class InZipDirectory : DirectoryBase<InternalZipDirectory>
    {
        private InternalZipDirectory _dic;
        private ZipFile _file;

        public InZipDirectory(IDirectory? parentDirectory, string originalPath, InternalZipDirectory dic, ZipFile? file, string name)
            : base(() => parentDirectory, originalPath, name)
        {
            _dic = Argument.NotNull(dic, nameof(dic));
            _file = Argument.NotNull(file, nameof(file));
        }

        public override DateTime LastModified => _dic.ZipEntry?.ModifiedTime ?? DateTime.MinValue;

        public override bool Exist => _dic.ZipEntry != null || _dic.Files.Count + _dic.Directorys.Count > 0;

        public override IDirectory GetDirectory(string name) 
            => throw new NotSupportedException();

        public override IEnumerable<IDirectory> Directories => _dic.Directorys.Select(internalZipDirectory
            => new InZipDirectory(this, OriginalPath.CombinePath(internalZipDirectory.Name), internalZipDirectory, _file, internalZipDirectory.Name));

        public override IEnumerable<IFile> Files => _dic.Files.Select(ent => new InZipFile(this, OriginalPath.CombinePath(InternalZipDirectory.GetFileName(ent)), _file, _dic, ent));

        protected override void DeleteImpl()
        {
            DeleteDic(_dic, _file);
        }

        private static void DeleteDic([NotNull] InternalZipDirectory dic, [NotNull] ZipFile file)
        {
            Argument.NotNull(dic, nameof(dic));
            Argument.NotNull(file, nameof(file));

            if (dic.ZipEntry != null)
                file.RemoveEntry(dic.ZipEntry);

            foreach (var zipEntry in dic.Files)
                file.RemoveEntry(zipEntry);

            foreach (var internalZipDirectory in dic.Directorys)
                DeleteDic(internalZipDirectory, file);
        }

        protected override InternalZipDirectory GetInfo(string path) 
            => _dic;

        public override IFile GetFile(string name)
        {
            var parts = name.Split(InternalZipDirectory.PathSplit, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length <= 1)
            {
                var compledetPath = OriginalPath.CombinePath(name);
                return new InZipFile(this, compledetPath, _file, _dic, _file[compledetPath]);
            }

            var dic = _dic;
            var inZipParent = this;

            var originalPath = new StringBuilder(OriginalPath);

            for (var i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1)
                    return new InZipFile(inZipParent, originalPath.Append('\\').Append(parts[i]).ToString(), _file, dic, _file[originalPath.ToString()]);

                dic = dic.GetOrAdd(parts[i]);
                originalPath.Append('\\').Append(parts[i]);
                inZipParent = new InZipDirectory(inZipParent, originalPath.ToString(), dic, _file, name);
            }

            throw new InvalidOperationException();
        }

        public override IDirectory MoveTo(string location)
        {
            throw new NotSupportedException();
        }

        protected void ResetDirectory([NotNull] ZipFile file, [NotNull] InternalZipDirectory directory)
        {
            _file = Argument.NotNull(file, nameof(file));
            _dic = Argument.NotNull(directory, nameof(directory));
        }
    }
}