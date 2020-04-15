using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Tauron
{
    [DebuggerStepThrough, PublicAPI]
    public static class IOExtensions
    {
        public static string PathShorten(this string path, int length)
        {
            var pathParts = path.Split('\\');
            var pathBuild = new StringBuilder(path.Length);
            var lastPart = pathParts[^1];
            var prevPath = "";

            //Erst prüfen ob der komplette String evtl. bereits kürzer als die Maximallänge ist
            if (path.Length >= length) return path;

            for (var i = 0; i < pathParts.Length - 1; i++)
            {
                pathBuild.Append(pathParts[i] + @"\");
                if ((pathBuild + @"...\" + lastPart).Length >= length) return prevPath;
                prevPath = pathBuild + @"...\" + lastPart;
            }

            return prevPath;
        }


        public static void Clear(this DirectoryInfo dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (!dic.Exists) return;

            foreach (var entry in dic.GetFileSystemInfos())
            {
                if (entry is FileInfo file)
                    file.Delete();
                else
                {
                    if (!(entry is DirectoryInfo dici)) continue;

                    Clear(dici);
                    dici.Delete();
                }
            }
        }

        public static void ClearDirectory(this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            Clear(new DirectoryInfo(dic));
        }


        public static void ClearParentDirectory(this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            ClearDirectory(Path.GetDirectoryName(dic));
        }


        public static string CombinePath(this string path, params string[] paths)
        {
            paths = paths.Select(str => str.TrimStart('\\')).ToArray();
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);

            var tempPath = Path.Combine(paths);
            return Path.Combine(path, tempPath);
        }


        public static string CombinePath(this string path, string path1)
            => string.IsNullOrWhiteSpace(path) ? path1 : Path.Combine(path, path1);

        [JetBrains.Annotations.NotNull]
        public static string CombinePath(this FileSystemInfo path, string path1) 
            => CombinePath(path.FullName, path1);

        public static void CopyFileTo(this string source, string destination)
        {
            if (!source.ExisFile()) return;

            File.Copy(source, destination, true);
        }

        public static bool CreateDirectoryIfNotExis(this string path)
        {
            try
            {
                if (!Path.HasExtension(path)) return CreateDirectoryIfNotExis(new DirectoryInfo(path));

                var temp = Path.GetDirectoryName(path);

                return CreateDirectoryIfNotExis(new DirectoryInfo(temp ?? throw new InvalidOperationException()));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool CreateDirectoryIfNotExis(this DirectoryInfo dic)
        {
            if (dic.Exists) return false;
            dic.Create();

            return true;
        }

        public static void SafeDelete(this FileSystemInfo info)
        {
            if (info.Exists) info.Delete();
        }

        public static void DeleteDirectory([JetBrains.Annotations.NotNull] this string path)
        {
            if (Path.HasExtension(path))
                path = Path.GetDirectoryName(path);

            try
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public static void DeleteDirectory(this string path, object sub)
        {
            var tempsub = sub.ToString();
            var compl = CombinePath(path, tempsub);
            if (Directory.Exists(compl)) Directory.Delete(compl);
        }

        public static void DeleteDirectory(this string path, bool recursive)
        {
            if (Directory.Exists(path)) 
                Directory.Delete(path, recursive);
        }

        public static void DeleteDirectoryIfEmpty(this string path)
        {
            if(!Directory.Exists(path)) return;
            if (!Directory.EnumerateFileSystemEntries(path).Any()) Directory.Delete(path);
        }

        public static void DeleteFile(this string path)
        {
            if (!path.ExisFile()) return;

            File.Delete(path);
        }

        public static bool DirectoryConainsInvalidChars(this string path)
        {
            var invalid = Path.GetInvalidPathChars();
            return path?.Any(invalid.Contains!) ?? true;
        }

        public static IEnumerable<string> EnumrateFileSystemEntries(this string dic) 
            => Directory.EnumerateFileSystemEntries(dic);

        public static IEnumerable<string> EnumerateAllFiles(this string dic) 
            => Directory.EnumerateFiles(dic, "*.*", SearchOption.AllDirectories);

        public static IEnumerable<string> EnumerateAllFiles(this string dic, string filter) 
            => Directory.EnumerateFiles(dic, filter, SearchOption.AllDirectories);

        public static IEnumerable<string> EnumerateDirectorys(this string path) 
            => !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateDirectories(path);

        public static IEnumerable<FileSystemInfo> EnumerateFileSystemEntrys(this string path) 
            => new DirectoryInfo(path).EnumerateFileSystemInfos();

        public static IEnumerable<string> EnumerateFiles(this string dic) 
            => Directory.EnumerateFiles(dic, "*.*", SearchOption.TopDirectoryOnly);

        public static IEnumerable<string> EnumerateFiles(this string dic, string filter) 
            => Directory.EnumerateFiles(dic, filter, SearchOption.TopDirectoryOnly);

        public static IEnumerable<string> EnumerateTextLinesIfExis(this string path)
        {
            if (!File.Exists(path)) yield break;

            using var reader = File.OpenText(path);

            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) yield break;

                yield return line;
            }
        }

        public static IEnumerable<string> EnumerateTextLines(this TextReader reader)
        {
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) yield break;

                yield return line;
            }
        }

        public static bool ExisDirectory(this string path) 
            => Directory.Exists(path);

        public static bool ExisFile(this string workingDirectory, string file)
        {
            try
            {
                return File.Exists(Path.Combine(workingDirectory, file));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool ExisFile(this string? file)
            => !string.IsNullOrWhiteSpace(file) && File.Exists(file);

        public static DateTime GetDirectoryCreationTime(this string path) 
            => Directory.GetCreationTime(path);

        public static string GetDirectoryName(this string path) 
            => Path.GetDirectoryName(path);

        public static string GetDirectoryName(this StringBuilder path) 
            => GetDirectoryName(path.ToString());


        public static string[] GetDirectorys(this string path) 
            => Directory.GetDirectories(path);

        public static string GetExtension(this string path) 
            => Path.GetExtension(path);

        public static string GetFileName(this string path) 
            => Path.GetFileName(path);

        public static string GetFileNameWithoutExtension(this string path) 
            => Path.GetFileNameWithoutExtension(path);

        public static int GetFileSystemCount(this string strDir) 
            => GetFileSystemCount(new DirectoryInfo(strDir));

        public static int GetFileSystemCount(this DirectoryInfo di)
        {
            var count = di.GetFiles().Length;
            
            // 2. Für alle Unterverzeichnisse im aktuellen Verzeichnis
            foreach (var diSub in di.GetDirectories())
            {
                // 2a. Statt Console.WriteLine hier die gewünschte Aktion
                count++;

                // 2b. Rekursiver Abstieg
                count += GetFileSystemCount(diSub);
            }

            return count;
        }

        public static string[] GetFiles(this string dic) 
            => Directory.GetFiles(dic);

        public static string[] GetFiles(this string path, string pattern, SearchOption option) 
            => Directory.GetFiles(path, pattern, option);

        public static string GetFullPath(this string path) 
            => Path.GetFullPath(path);

        public static bool HasExtension(this string path) 
            => Path.HasExtension(path);

        public static bool IsPathRooted(this string path) 
            => Path.IsPathRooted(path);

        public static void MoveTo(this string source, string dest) 
            => File.Move(source, dest);

        public static void MoveTo(this string source, string workingDirectory, string dest)
        {
            var realDest = dest;

            if (!dest.HasExtension())
            {
                var fileName = Path.GetFileName(source);
                realDest = Path.Combine(dest, fileName);
            }

            var realSource = Path.Combine(workingDirectory, source);

            File.Move(realSource, realDest);
        }

        public static Stream OpenRead(this string path, FileShare share)
        {
            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, share);
        }

        public static Stream OpenRead(this string path) 
            => OpenRead(path, FileShare.None);

        public static StreamWriter OpenTextAppend(this string path)
        {
            path.CreateDirectoryIfNotExis();
            return new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None));
        }

        public static StreamReader OpenTextRead(this string path) 
            => File.OpenText(path);

        public static StreamWriter OpenTextWrite(this string path)
        {
            path.CreateDirectoryIfNotExis();
            return new StreamWriter(path);
        }

        public static Stream OpenWrite(this string path, bool delete = true) 
            => OpenWrite(path, FileShare.None, delete);

        public static Stream OpenWrite(this string path, FileShare share, bool delete = true)
        {
            if (delete)
                path.DeleteFile();

            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, share);
        }

        public static byte[] ReadAllBytesIfExis(this string path) 
            => !File.Exists(path) ? new byte[0] : File.ReadAllBytes(path);

        public static byte[] ReadAllBytes(this string path) 
            => File.ReadAllBytes(path);

        public static string ReadTextIfExis(this string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        public static string ReadTextIfExis(this string workingDirectory, string subPath) 
            => ReadTextIfExis(CombinePath(workingDirectory, subPath));

        public static IEnumerable<string> ReadTextLinesIfExis(this string path)
        {
            if (!File.Exists(path)) yield break;

            using var reader = File.OpenText(path);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;

                yield return line;
            }
        }

        public static bool TryCreateUriWithoutScheme(this string str, [MaybeNullWhen(false)]out Uri? uri, params string[] scheme)
        {
            var flag = Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var target);

            // ReSharper disable once AccessToModifiedClosure
            if (flag)
            {
                foreach (var s in scheme.Where(s => flag))
                    flag = target.Scheme != s;
            }

            uri = flag ? target : null;

            return flag;
        }

        public static void WriteTextContentTo(this string content, string path) 
            => File.WriteAllText(path, content);


        public static void WriteTextContentTo(this string content, string workingDirectory, string path) 
            => WriteTextContentTo(content, CombinePath(workingDirectory, path));
    }
}