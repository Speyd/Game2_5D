using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureLib
{
    /// <summary>Class for path management</summary>
    public static class ResourceManager
    {
        /// <summary>The path to the file, which is defined manually</summary>
        public static string RootDirectory { get; private set; } = FindProjectRoot();
        /// <summary>Path to the main directory(where the .sln file is located)</summary>
        public static string MainDirectory { get; } = FindProjectRoot();

        private const string solutionFilePath = "*.sln";
        private const string projectFilePath = "*.csproj";

        /// <summary>
        /// Returns the combined path of the specified home directory path and resource path.
        /// </summary>
        /// <param name="pathResource">Path to resource</param>
        /// <returns>Resource path</returns>
        public static string GetPath(string pathResource)
        {
            return Path.Combine(RootDirectory, pathResource);
        }
        /// <summary>
        /// Returns the combined path of the main directory and the resource path.
        /// </summary>
        /// <param name="pathResource">Path to resource</param>
        /// <returns>Resource path</returns>
        public static string GetMainPath(string pathResource)
        {
            return Path.Combine(MainDirectory, pathResource);
        }
        /// <summary>
        /// Returns the combined directory path with the ".csproj" file and the resource path
        /// </summary>
        /// <param name="pathDirectory">Directory path where need to find ".csproj"</param>
        /// <returns>Directory path</returns>
        public static string GetPathCurrentDirectory(string pathDirectory)
        {
            return FindCSProjRoot(pathDirectory);
        }
        /// <summary>
        /// Checks whether the specified directory exists or not.
        /// </summary>
        /// <param name="pathRootDirectory">Path to the root directory</param>
        public static void CheckTrueRootDirectoryPath(string pathRootDirectory)
        {
            if (string.IsNullOrWhiteSpace(pathRootDirectory))
            {
                throw new ArgumentException("The path cannot be empty.");
            }
            if (!Directory.Exists(pathRootDirectory))
            {
                throw new DirectoryNotFoundException($"Directory on the way {pathRootDirectory} does not exist.");
            }

            char directorySeparator = Path.DirectorySeparatorChar;
            if (!pathRootDirectory.StartsWith(directorySeparator.ToString()))
            {
                throw new ArgumentException($"The path must start from the root directory. ({directorySeparator}).");
            }
        }
        /// <summary>
        /// Sets the RootDirectory property to a new directory.
        /// </summary>
        /// <param name="pathRootDirectory">Path to the root directory</param>
        public static void SetRootDirectoryPath(string pathRootDirectory)
        {
            CheckTrueRootDirectoryPath(pathRootDirectory);
            RootDirectory = pathRootDirectory;
        }

        static string FindProjectRoot()
        {
            DirectoryInfo? dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (dir != null)
            {
                if (dir.GetFiles(solutionFilePath).Any())
                    return dir.FullName + Path.DirectorySeparatorChar.ToString();

                dir = dir.Parent;
            }

            throw new Exception("Project root folder not found");
        }
        static string FindCSProjRoot(string pathDirectory)
        {
            DirectoryInfo? dir = new DirectoryInfo(pathDirectory);

            while (dir != null)
            {
                if (dir.GetFiles(projectFilePath).Any())
                    return dir.FullName + Path.DirectorySeparatorChar.ToString();

                dir = dir.Parent;
            }

            throw new Exception("Project root folder not found");
        }
    }
}
