using System;
using System.Collections.Generic;
using System.IO;

namespace Akka.Copy.Core
{
    public static class Extensions
    {
        public static IEnumerable<TType> ErrorProof<TType>(this IEnumerable<TType> enumerable, Action<Exception> errorHandler)
            => new ErrorProofEnumerable<TType>(enumerable, errorHandler);

        public static string Gigabytes(this long bytes)
        {
            return $"{(bytes / 1024d / 1024d / 1024d):F3} GB";
        }

        public static IEnumerable<string> TraverseTree(this string root, Action<Exception> errorHandler)
        {
            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>(20);

            if (!Directory.Exists(root))
            {
                throw new ArgumentException();
            }

            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string   currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run.
                catch (UnauthorizedAccessException e)
                {
                    errorHandler(e);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    errorHandler(e);
                    continue;
                }

                string[] files;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    errorHandler(e);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    errorHandler(e);
                    continue;
                }

                // Perform the required action on each file here.
                // Modify this block to perform your required task.
                foreach (string file in files)
                {
                    yield return file;
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }
        }
    }
}