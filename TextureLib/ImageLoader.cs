using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace TextureLib
{
    /// <summary>Uploads and checks images</summary>
    public static class ImageLoader
    {
        /// <summary>A dictionary of image types (which only store one frame in themselves) and a method that will be called when it is added</summary>
        public static Dictionary<string, Func<string, TextureObstacle>> frameExtensions;
        /// <summary>A dictionary of image types (which several frames store in themselves) and a method that will be called when it is added</summary>
        public static Dictionary<string, Func<string, List<TextureObstacle>>> multiFrameExtensions;

        static ImageLoader()
        {
            frameExtensions = new()
            {
                { ".jpg", LoadFrame },
                { ".jpeg", LoadFrame },
                { ".png", LoadFrame },
                { ".bmp", LoadFrame },
                { ".tiff", LoadFrame },
                { ".webp", LoadFrame },
            };

            multiFrameExtensions = new()
            {
                { ".gif", LoadFrames },
            };
        }


        private static bool IsImageFile(string[] typeFrameExtensions, string path)
        {
            string extension = Path.GetExtension(path)?.ToLower() ?? "";

            return typeFrameExtensions.Contains(extension);
        }
        /// <summary>They will check the path for existence and belonging to the image type</summary>
        public static void IsTrueImagePath(string path)
        {
            if (!IsImageFile(frameExtensions.Keys.ToArray(), path) &&
                !IsImageFile(multiFrameExtensions.Keys.ToArray(), path))
            {
                throw new Exception("Error file extensions(non photo or texture)");
            }
            else if (!File.Exists(path))
                throw new Exception("Error path TextureObstacle");
        }

        /// <summary>Adds an object to a path that has 1 frame</summary>
        /// /// <summary>
        /// Adds an object to a path that has 1 frame
        /// </summary>
        /// <param name="path">File paths</param>
        /// <returns>Textures that were obtained from the path</returns>
        public static TextureObstacle LoadFrame(string path)
        {
            IsTrueImagePath(path);

            return new TextureObstacle(path);
        }
        /// <summary>
        /// Adds an object to a path that has more than 1 frame
        /// </summary>
        /// <param name="path">File paths</param>
        /// <returns>List of textures that were obtained from the path</returns>
        public static List<TextureObstacle> LoadFrames(string path)
        {
            IsTrueImagePath(path);
            List<TextureObstacle> frames = new();


            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                using (SixLabors.ImageSharp.Image gifImage = SixLabors.ImageSharp.Image.Load(path))
                {
                    int frameCount = gifImage.Frames.Count;

                    for (int i = 0; i < frameCount; i++)
                    {
                        using (var frame = gifImage.Frames.CloneFrame(i))
                        {
                            string filePath = Path.Combine(tempDir, $"frame_{i}.png");
                            frame.SaveAsPng(filePath);

                            var texture = new SFML.Graphics.Texture(filePath);
                            frames.Add(new TextureObstacle(texture));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when enabling multi frame type: {ex.Message}");
            }
            finally
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }

            return frames;
        }

        /// <summary>
        /// Getting textures using path
        /// </summary>
        /// <param name="path">File paths</param>
        /// <returns>List of textures that were obtained from the path</returns>
        public static List<TextureObstacle>? TextureLoad(string path)
        {
            frameExtensions.TryGetValue(Path.GetExtension(path)?.ToLower() ?? "", out var singleFrameLoader);
            TextureObstacle? frame = singleFrameLoader?.Invoke(path);

            multiFrameExtensions.TryGetValue(Path.GetExtension(path)?.ToLower() ?? "", out var multiFrameLoader);
            List<TextureObstacle>? frames = multiFrameLoader?.Invoke(path);

            return frame is null ? frames : new() { frame };
        }
        /// <summary>
        /// Getting textures using path
        /// </summary>
        /// <param name="paths">List file paths</param>
        /// <returns>List of textures that were obtained from the path</returns>
        public static List<TextureObstacle> TexturesLoad(List<string> paths)
        {
            List<TextureObstacle> frames = new();
            foreach (var path in paths)
            {
                var frame = TextureLoad(path);

                if (frame is not null)
                    frames.AddRange(frame);
            }

            return frames;
        }
        /// <summary>
        /// Getting textures using path
        /// </summary>
        /// <param name="paths">File paths</param>
        /// <returns>List of textures that were obtained from the path</returns>
        public static List<TextureObstacle> TexturesLoad(params string[] paths)
        {
            return TexturesLoad(paths.ToList());        
        }
        /// <summary>
        /// Getting textures using path
        /// </summary>
        /// <param name="path">Folder path</param>
        /// <param name="folderAccounting">true - go through all subfolders in the given directory, false - go through only the given directory</param>
        /// <returns>List of textures that were obtained from the path</returns>
        public static List<TextureObstacle> TexturesLoadFromFolder(string path, bool folderAccounting)
        {
            if (!Directory.Exists(path))
                throw new Exception("Error path TextureObstacle");

            string[] files = Directory.GetFiles(path);
            string[] directories = Directory.GetDirectories(path);

            if (folderAccounting)
            {
                foreach (var directorie in directories)
                {
                    TexturesLoadFromFolder(directorie, folderAccounting);
                }
            }

            List<TextureObstacle> frames = new();
            foreach (var file in files)
            {
                IsTrueImagePath(file);
                var texture = TextureLoad(file);
                if(texture is not null)
                    frames.AddRange(texture);
            }

            return frames;
        }
    }
}
