#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NLog;

#endregion

namespace Tauron
{
    /// <summary>The io extensions.</summary>
    [PublicAPI]
    public static class IOExtensions
    {
        #region Public Methods and Operators

        [NotNull]
        public static string PathShorten([NotNull] this string path, int length)
        {
            var pathParts = path.Split('\\');
            var pathBuild = new StringBuilder(path.Length);
            var lastPart = pathParts[pathParts.Length - 1];
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

        /// <summary>
        ///     The clear.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        public static void Clear([NotNull] this DirectoryInfo dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (!dic.Exists) return;

            foreach (var entry in dic.GetFileSystemInfos())
            {
                var file = entry as FileInfo;
                if (file != null)
                {
                    file.Delete();
                }
                else
                {
                    var dici = entry as DirectoryInfo;
                    if (dici == null) continue;

                    Clear(dici);
                    dici.Delete();
                }
            }
        }

        /// <summary>
        ///     The clear directory.
        /// </summary>
        /// +
        /// <param name="dic">
        ///     The dic.
        /// </param>
        public static void ClearDirectory([NotNull] this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            Clear(new DirectoryInfo(dic));
        }

        /// <summary>
        ///     The clear parent directory.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        public static void ClearParentDirectory([NotNull] this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            ClearDirectory(Path.GetDirectoryName(dic));
        }

        /// <summary>
        ///     The combine path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="paths">
        ///     The paths.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string CombinePath([NotNull] this string path, [ItemNotNull] [NotNull] params string[] paths)
        {
            if (paths == null) throw new ArgumentNullException(nameof(paths));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            paths = paths.Select(str => str.TrimStart('\\')).ToArray();

            if (Path.HasExtension(path)) path = Path.GetDirectoryName(path);

            var tempPath = Path.Combine(paths);
            return Path.Combine(path, tempPath);
        }

        /// <summary>
        ///     The combine path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="path1">
        ///     The path 1.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string CombinePath([NotNull] this string path, [NotNull] string path1)
        {
            if (path1 == null) throw new ArgumentNullException(nameof(path1));
            if (string.IsNullOrWhiteSpace(path)) return path1;
            //if (Path.HasExtension(path)) path = Path.GetDirectoryName(path);

            return Path.Combine(path ?? throw new ArgumentNullException(nameof(path)), path1);
        }

        /// <summary>
        ///     The combine path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="path1">
        ///     The path 1.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string CombinePath([NotNull] this FileSystemInfo path, [NotNull] string path1)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (path1 == null) throw new ArgumentNullException(nameof(path1));
            return CombinePath(path.FullName, path1);
        }

        /// <summary>
        ///     The copy file to.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        public static void CopyFileTo([NotNull] this string source, [NotNull] string destination)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            if (!source.ExisFile()) return;

            File.Copy(source, destination, true);
        }

        /// <summary>
        ///     The create directory if not exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CreateDirectoryIfNotExis([NotNull] this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
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

        /// <summary>
        ///     The create directory if not exis.
        /// </summary>
        /// <param name="dic">
        ///     The dic.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool CreateDirectoryIfNotExis([NotNull] this DirectoryInfo dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            if (dic.Exists) return false;

            dic.Create();

            return true;
        }

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        public static void Delete([NotNull] this FileSystemInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (info.Exists) info.Delete();
        }

        /// <summary>
        ///     The delete directory.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void DeleteDirectory([NotNull] this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

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

        /// <summary>
        ///     The delete directory.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="sub">
        ///     The sub.
        /// </param>
        public static void DeleteDirectory([NotNull] this string path, [NotNull] object sub)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (sub == null) throw new ArgumentNullException(nameof(sub));
            var compl = CombinePath(path, sub.ToString());
            if (Directory.Exists(compl)) Directory.Delete(compl);
        }

        public static void DeleteDirectory([NotNull] this string path, bool recursive)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            if (Directory.Exists(path)) Directory.Delete(path, recursive);
        }

        /// <summary>
        ///     The delete directory if empty.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void DeleteDirectoryIfEmpty([NotNull] this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!Directory.EnumerateFileSystemEntries(path).GetEnumerator().MoveNext()) Directory.Delete(path);
        }

        /// <summary>
        ///     The delete file.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void DeleteFile([NotNull] this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!path.ExisFile()) return;

            File.Delete(path);
        }

        public static bool DirectoryConainsInvalidChars([NotNull] this string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var invalid = Path.GetInvalidPathChars();

            return path.All(invalid.Contains);
        }

        [NotNull]
        public static IEnumerable<string> EnumrateFileSystemEntries([NotNull] this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            return Directory.EnumerateFileSystemEntries(dic);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateAllFiles([NotNull] this string dic)
        {
            if (dic == null) throw new ArgumentNullException(nameof(dic));
            return Directory.EnumerateFiles(dic, "*.*", SearchOption.AllDirectories);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateAllFiles([NotNull] this string dic, [NotNull] string filter)
        {
            if (string.IsNullOrWhiteSpace(dic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dic));
            if (string.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filter));
            return Directory.EnumerateFiles(dic, filter, SearchOption.AllDirectories);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateDirectorys([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateDirectories(path);
        }

        [NotNull]
        public static IEnumerable<FileSystemInfo> EnumerateFileSystemEntrys([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return new DirectoryInfo(path).EnumerateFileSystemInfos();
        }

        [NotNull]
        public static IEnumerable<string> EnumerateFiles([NotNull] this string dic)
        {
            if (string.IsNullOrWhiteSpace(dic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dic));
            return Directory.EnumerateFiles(dic, "*.*", SearchOption.TopDirectoryOnly);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateFiles([NotNull] this string dic, [NotNull] string filter)
        {
            if (string.IsNullOrWhiteSpace(dic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dic));
            if (string.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filter));
            return Directory.EnumerateFiles(dic, filter, SearchOption.TopDirectoryOnly);
        }

        [NotNull]
        public static IEnumerable<string> EnumerateTextLinesIfExis([NotNull] this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            if (!File.Exists(path)) yield break;

            using (var reader = File.OpenText(path))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) yield break;

                    yield return line;
                }
            }
        }

        [NotNull]
        public static IEnumerable<string> EnumerateTextLines([NotNull] this TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) yield break;

                yield return line;
            }
        }

        /// <summary>
        ///     The exis directory.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ExisDirectory([NotNull] this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("Value cannot be null or empty.", nameof(path));
            return Directory.Exists(path);
        }

        /// <summary>
        ///     The exis file.
        /// </summary>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="file">
        ///     The file.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ExisFile([NotNull] this string workingDirectory, [NotNull] string file)
        {
            if (string.IsNullOrEmpty(workingDirectory))
                throw new ArgumentException("Value cannot be null or empty.", nameof(workingDirectory));
            if (string.IsNullOrEmpty(file)) throw new ArgumentException("Value cannot be null or empty.", nameof(file));
            try
            {
                return File.Exists(Path.Combine(workingDirectory, file));
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        ///     The exis file.
        /// </summary>
        /// <param name="file">
        ///     The file.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool ExisFile([NotNull] this string file)
        {
            return !string.IsNullOrWhiteSpace(file) && File.Exists(file);
        }

        /// <summary>
        ///     The get directory creation time.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="DateTime" />.
        /// </returns>
        public static DateTime GetDirectoryCreationTime([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Directory.GetCreationTime(path);
        }

        /// <summary>
        ///     The get directory name.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetDirectoryName([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        ///     The get directory name.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetDirectoryName([NotNull] this StringBuilder path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return GetDirectoryName(path.ToString());
        }


        [NotNull]
        public static string[] GetDirectorys([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Directory.GetDirectories(path);
        }

        /// <summary>
        ///     The get extension.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetExtension([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.GetExtension(path);
        }

        /// <summary>
        ///     The get file name.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetFileName([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.GetFileName(path);
        }

        /// <summary>
        ///     The get file name without extension.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetFileNameWithoutExtension([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        ///     The get file system count.
        /// </summary>
        /// <param name="strDir">
        ///     The str dir.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetFileSystemCount([NotNull] this string strDir)
        {
            if (string.IsNullOrWhiteSpace(strDir))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(strDir));
            // 0. Einstieg in die Rekursion auf oberster Ebene
            return GetFileSystemCount(new DirectoryInfo(strDir));
        }

        /// <summary>
        ///     The get file system count.
        /// </summary>
        /// <param name="di">
        ///     The di.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public static int GetFileSystemCount([NotNull] this DirectoryInfo di)
        {
            if (di == null) throw new ArgumentNullException(nameof(di));
            var count = 0;

            try
            {
                // 1. Für alle Dateien im aktuellen Verzeichnis
                count += di.GetFiles().Count();

                // 2. Für alle Unterverzeichnisse im aktuellen Verzeichnis
                foreach (var diSub in di.GetDirectories())
                {
                    // 2a. Statt Console.WriteLine hier die gewünschte Aktion
                    count++;

                    // 2b. Rekursiver Abstieg
                    count += GetFileSystemCount(diSub);
                }
            }
            catch (Exception e)
            {
                // 3. Statt Console.WriteLine hier die gewünschte Aktion
                LogManager.GetLogger(nameof(IOExtensions), typeof(IOExtensions)).Error(e);
                throw;
            }

            return count;
        }

        [NotNull]
        public static string[] GetFiles([NotNull] this string dic)
        {
            if (string.IsNullOrWhiteSpace(dic))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dic));
            return Directory.GetFiles(dic);
        }

        [NotNull]
        public static string[] GetFiles([NotNull] this string path, [NotNull] string pattern, SearchOption option)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(pattern));
            return Directory.GetFiles(path, pattern, option);
        }

        /// <summary>
        ///     The get full path.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string GetFullPath([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.GetFullPath(path);
        }

        /// <summary>
        ///     The has extension.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool HasExtension([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.HasExtension(path);
        }

        /// <summary>
        ///     The is path rooted.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsPathRooted([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return Path.IsPathRooted(path);
        }

        /// <summary>
        ///     The move to.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="dest">
        ///     The dest.
        /// </param>
        public static void MoveTo([NotNull] this string source, [NotNull] string dest)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(source));
            if (string.IsNullOrWhiteSpace(dest))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dest));
            File.Move(source, dest);
        }

        /// <summary>
        ///     The move to.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="dest">
        ///     The dest.
        /// </param>
        public static void MoveTo([NotNull] this string source, [NotNull] string workingDirectory,
            [NotNull] string dest)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(source));
            if (string.IsNullOrWhiteSpace(workingDirectory))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(workingDirectory));
            if (string.IsNullOrWhiteSpace(dest))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(dest));
            var realDest = dest;

            if (!dest.HasExtension())
            {
                var fileName = Path.GetFileName(source);
                realDest = Path.Combine(dest, fileName);
            }

            var realSource = Path.Combine(workingDirectory, source);

            File.Move(realSource, realDest);
        }

        /// <summary>
        ///     The open read.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <param name="share">
        ///     The share.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        [NotNull]
        public static Stream OpenRead([NotNull] this string path, FileShare share)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, share);
        }

        /// <summary>
        ///     The open read.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        [NotNull]
        public static Stream OpenRead([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return OpenRead(path, FileShare.None);
        }

        /// <summary>
        ///     The open text append.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="StreamWriter" />.
        /// </returns>
        [NotNull]
        public static StreamWriter OpenTextAppend([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            path.CreateDirectoryIfNotExis();
            return new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None));
        }

        /// <summary>
        ///     The open text read.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="StreamReader" />.
        /// </returns>
        [NotNull]
        public static StreamReader OpenTextRead([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return File.OpenText(path);
        }

        /// <summary>
        ///     The open text write.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="StreamWriter" />.
        /// </returns>
        [NotNull]
        public static StreamWriter OpenTextWrite([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            path.CreateDirectoryIfNotExis();
            return new StreamWriter(path);
        }

        [NotNull]
        public static Stream OpenWrite([NotNull] this string path, bool delete = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return OpenWrite(path, FileShare.None, delete);
        }

        [NotNull]
        public static Stream OpenWrite([NotNull] this string path, FileShare share, bool delete = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (delete)
                path.DeleteFile();

            path = path.GetFullPath();
            path.CreateDirectoryIfNotExis();
            return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, share);
        }

        [NotNull]
        public static byte[] ReadAllBytesIfExis([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return !File.Exists(path) ? new byte[0] : File.ReadAllBytes(path);
        }

        [NotNull]
        public static byte[] ReadAllBytes([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return File.ReadAllBytes(path);
        }

        /// <summary>
        ///     The read text if exis.
        /// </summary>
        /// <param name="path">
        ///     The path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string ReadTextIfExis([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        /// <summary>
        ///     The read text if exis.
        /// </summary>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="subPath">
        ///     The sub path.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [NotNull]
        public static string ReadTextIfExis([NotNull] this string workingDirectory, [NotNull] string subPath)
        {
            if (string.IsNullOrWhiteSpace(workingDirectory))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(workingDirectory));
            if (string.IsNullOrWhiteSpace(subPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(subPath));
            return ReadTextIfExis(CombinePath(workingDirectory, subPath));
        }

        [NotNull]
        public static IEnumerable<string> ReadTextLinesIfExis([NotNull] this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (!File.Exists(path)) yield break;

            using (var reader = File.OpenText(path))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;

                    yield return line;
                }
            }
        }

        /// <summary>
        ///     The create uri without scheme.
        /// </summary>
        /// <param name="str">
        ///     The str.
        /// </param>
        /// <param name="uri">
        ///     The uri.
        /// </param>
        /// <param name="scheme">
        ///     The scheme.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool TryCreateUriWithoutScheme([NotNull] this string str, out Uri uri,
            [NotNull] params string[] scheme)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(str));
            Uri target;
            var flag = Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out target);

// ReSharper disable once AccessToModifiedClosure
            if (flag)
                foreach (var s in scheme.Where(s => flag))
                    flag = target.Scheme != s;

            uri = flag ? target : null;

            return flag;
        }

        /// <summary>
        ///     The write text content to.
        /// </summary>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void WriteTextContentTo([NotNull] this string content, [NotNull] string path)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(content));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            File.WriteAllText(path, content);
        }

        /// <summary>
        ///     The write text content to.
        /// </summary>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <param name="workingDirectory">
        ///     The working directory.
        /// </param>
        /// <param name="path">
        ///     The path.
        /// </param>
        public static void WriteTextContentTo([NotNull] this string content, [NotNull] string workingDirectory,
            [NotNull] string path)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(content));
            if (string.IsNullOrWhiteSpace(workingDirectory))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(workingDirectory));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            WriteTextContentTo(content, CombinePath(workingDirectory, path));
        }

        #endregion
    }
}