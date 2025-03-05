using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureLib
{
    public static class ResourceManager
    {
        public static string RootDirectory { get; private set; } = FindProjectRoot();
        private const string solutionFilePath = "*.sln";

        public static string GetPath(string pathResource)
        {
            return Path.Combine(RootDirectory, pathResource);
        }
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
    }
}
