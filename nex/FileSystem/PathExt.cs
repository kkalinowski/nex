using lib12.Collections;
using System;
using System.IO;

namespace nex.FileSystem
{
    /// <summary>
    /// Extends System.IO.Path
    /// </summary>
    public static class PathExt
    {
        /// <summary>
        /// Determines if path is to root of drive
        /// </summary>
        /// <param name="dir">Path to check</param>
        /// <returns>True if path is to root of drive</returns>
        public static bool IsDriveRoot(string dir)
        {
            return (dir == Path.GetPathRoot(dir));
        }

        /// <summary>
        /// Returns the letter of drive
        /// </summary>
        /// <param name="path">Path to examine</param>
        /// <returns>Drive letter</returns>
        public static string GetDriveLetter(string path)
        {
            return Path.GetPathRoot(path);
        }

        /// <summary>
        /// Return extension of file without a dot [.] in front of it
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns>Extension of path without dot</returns>
        public static string GetExtensionWithoutDot(string path)
        {
            var ext = Path.GetExtension(path);
            return ext.IsNotNullAndNotEmpty() ? ext.Substring(1) : ext;
        }

        /// <summary>
        /// Determines if path leads to real directory
        /// </summary>
        /// <param name="path">path to examine</param>
        /// <returns>True if directory realy exist</returns>
        public static bool IsPathToDirectory(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Returns invariant DirectoryName, not depending on system 
        /// </summary>
        /// <param name="path">Path to examine</param>
        /// <param name="isWindowsPath">Determines if it is normal windows path or not</param>
        /// <returns>Path to directory</returns>
        public static string GetDirectoryName(string path, bool isWindowsPath)
        {
            if (isWindowsPath)
                return Path.GetDirectoryName(path);
            else
            {
                //remove last occurence if exist
                if (path.EndsWith("/"))
                    path = path.Substring(0, path.Length - 1);

                //find separator
                int seperator = path.LastIndexOf("/");
                return path.Substring(0, seperator) + "/";//return substring with directory name
            }
        }

        /// <summary>
        /// Retrieve file or directory name from path
        /// </summary>
        /// <param name="path">Path to examine</param>
        /// <returns>Directory or file name</returns>
        public static string GetName(string path)
        {
            var parts = path.Split(new string[] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);
            return parts.IsEmpty() ? "/" : parts[parts.Length - 1];
        }

        /// <summary>
        /// Returns invariant combination of two paths, not depending on system
        /// </summary>
        /// <param name="path1">First path</param>
        /// <param name="path2">Second path</param>
        /// <param name="isWindowsPath">Determines if it is normal windows path or not</param>
        /// <returns>Combination of two paths</returns>
        public static string Combine(string path1, string path2, bool isWindowsPath)
        {
            if (isWindowsPath)
                return Path.Combine(path1, path2);
            else
            {
                if (!path1.EndsWith("/"))
                    return string.Concat(path1, "/", path2);
                else
                    return string.Concat(path1, path2);
            }
        }
    }
}