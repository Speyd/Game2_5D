using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureLib.Loader;
/// <summary>
/// Provides methods for managing and retrieving project resource paths,
/// such as the solution root directory, project directories, and resource files.
/// </summary>
public static class PathResolver
{
    private static readonly string solutionFilePattern = "*.sln";
    private static readonly string projectFilePattern = "*.csproj";

    /// <summary>
    /// Gets the root directory path where the solution (.sln) file is located.
    /// </summary>
    public static string RootDirectory { get; private set; } = FindRoot(solutionFilePattern);

    /// <summary>
    /// Gets the main directory path where the solution (.sln) file is located.
    /// Usually same as <see cref="RootDirectory"/>.
    /// </summary>
    public static string MainDirectory { get; } = FindRoot(solutionFilePattern);

    /// <summary>
    /// Combines the <see cref="RootDirectory"/> with a relative resource path.
    /// </summary>
    /// <param name="pathResource">The relative path to the resource.</param>
    /// <returns>The combined absolute path to the resource under the root directory.</returns>
    public static string GetPath(string pathResource) =>
        Path.Combine(RootDirectory, pathResource);

    /// <summary>
    /// Combines the <see cref="MainDirectory"/> with a relative resource path.
    /// </summary>
    /// <param name="pathResource">The relative path to the resource.</param>
    /// <returns>The combined absolute path to the resource under the main directory.</returns>
    public static string GetMainPath(string pathResource) =>
        Path.Combine(MainDirectory, pathResource);

    /// <summary>
    /// Finds and returns the directory path containing a project file (.csproj),
    /// starting from the specified directory and moving up the directory tree.
    /// </summary>
    /// <param name="pathDirectory">The starting directory path to search from.</param>
    /// <returns>The full path to the directory containing the project file.</returns>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if no directory containing a .csproj file is found in the path or its parents.
    /// </exception>
    public static string GetPathCurrentDirectory(string pathDirectory) =>
        FindRoot(projectFilePattern, pathDirectory);

    /// <summary>
    /// Validates whether the specified directory path exists and is not null or whitespace.
    /// </summary>
    /// <param name="pathRootDirectory">The directory path to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the path is null, empty, or whitespace.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
    public static void CheckValidDirectoryPath(string pathRootDirectory)
    {
        if (string.IsNullOrWhiteSpace(pathRootDirectory))
            throw new ArgumentException("The path cannot be null or whitespace.");

        if (!Directory.Exists(pathRootDirectory))
            throw new DirectoryNotFoundException($"Directory does not exist: {pathRootDirectory}");
    }

    /// <summary>
    /// Sets a new root directory path after validation.
    /// </summary>
    /// <param name="pathRootDirectory">The new root directory path.</param>
    /// <exception cref="ArgumentException">Thrown if the path is null, empty, or whitespace.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
    public static void SetRootDirectoryPath(string pathRootDirectory)
    {
        CheckValidDirectoryPath(pathRootDirectory);
        RootDirectory = pathRootDirectory;
    }

    /// <summary>
    /// Searches for a directory containing files matching the specified pattern,
    /// starting from an optional start directory and moving up through parent directories.
    /// </summary>
    /// <param name="searchPattern">The file search pattern (e.g., "*.sln", "*.csproj").</param>
    /// <param name="startDirectory">
    /// The directory to start searching from. Defaults to the application's base directory if null.
    /// </param>
    /// <returns>The full path to the directory containing matching files.</returns>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown if no directory containing matching files is found.
    /// </exception>
    private static string FindRoot(string searchPattern, string? startDirectory = null)
    {
        DirectoryInfo? dir = new DirectoryInfo(startDirectory ?? AppDomain.CurrentDomain.BaseDirectory);

        while (dir != null)
        {
            if (dir.GetFiles(searchPattern).Any())
                return dir.FullName + Path.DirectorySeparatorChar;

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException($"No folder containing {searchPattern} was found.");
    }
}
