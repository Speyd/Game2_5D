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
    private static string? _searchPattern = null;
    /// <summary>
    /// Gets or sets the file search pattern (e.g., "*.json", "*.csproj") 
    /// used to locate the root directory.
    /// When a new non-null value is set, the <see cref="RootDirectory"/> 
    /// is automatically updated by searching for the specified pattern.
    /// </summary>
    public static string? SearchPattern 
    {
        get => _searchPattern;
        set
        {
            if (value is null)
                return;

            _searchPattern = value;
            RootDirectory = FindRoot(value);
        }
    }


    private static string? _rootDirectory = null;
    /// <summary>
    /// Gets the root directory path where the solution (.sln) file is located.
    /// </summary>
    public static string RootDirectory 
    {
        get => _rootDirectory ?? throw new InvalidOperationException("RootDirectory is not initialized.");
        set => _rootDirectory = value;
    }

    /// <summary>
    /// Combines a root directory with a relative resource path.
    /// </summary>
    /// <param name="pathResource">
    /// The relative path to the resource.
    /// </param>
    /// <param name="searchPattern">
    /// An optional file search pattern (e.g., "*.json", "*.csproj").
    /// If specified, the method searches for the nearest parent directory
    /// that contains a file matching the pattern and uses it as the root.
    /// If <c>null</c>, the <see cref="RootDirectory"/> is used instead.
    /// </param>
    /// <returns>
    /// The combined absolute path to the resource under the resolved root directory.
    /// </returns>

    public static string GetPath(string pathResource, string? searchPattern = null) =>
        Path.Combine(searchPattern is null? RootDirectory : FindRoot(searchPattern), pathResource);




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
    public static string FindRoot(string searchPattern, string? startDirectory = null)
    {
        DirectoryInfo? dir = new DirectoryInfo(startDirectory ?? AppDomain.CurrentDomain.BaseDirectory);

        while (dir != null)
        {
            if (dir.GetDirectories(searchPattern).Any() || dir.GetFiles(searchPattern).Any())
                return dir.FullName + Path.DirectorySeparatorChar;

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException($"No folder containing {searchPattern} was found.");
    }
}
