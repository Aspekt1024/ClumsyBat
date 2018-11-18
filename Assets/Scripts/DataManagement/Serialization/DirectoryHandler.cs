using System.IO;

namespace ClumsyBat.DataManagement.Serialization
{
    /// <summary>
    /// Handles the retrieval of file paths and creates folders if required
    /// </summary>
    internal static class DirectoryHandler
    {
        public enum DirectoryCategory
        {
            Data
        }

        public enum FileType
        {
            dat
        }

        internal static string GetFullPath(string file, DirectoryCategory category, FileType fileType)
        {
            string path = GameStatics.Data.PersistentDataPath;
            if (!Directory.Exists(path))
            {
                throw new System.Exception("The path does not exist: " + path);
            }

            file = $"{category.ToString()}/{file}.{fileType.ToString()}";

            if (path.EndsWith("/") || path.EndsWith("\\"))
            {
                path.Remove(path.Length - 1, 1);
            }

            // The file can contain subfolders
            string[] fileStrings = file.Split('/', '\\');
            if (string.IsNullOrEmpty(fileStrings[fileStrings.Length - 1]))
            {
                throw new System.Exception("The filename is empty: " + file);
            }

            if (!fileStrings[fileStrings.Length - 1].Contains("."))
            {
                throw new System.Exception("The filename is missing an extension: " + file);
            }

            for (int i = 0; i < fileStrings.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(fileStrings[i])) continue;

                path += $"/{fileStrings[i]}";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            return path + $"/{fileStrings[fileStrings.Length - 1]}";
        }
    }
}
