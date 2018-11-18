using ClumsyBat.DataManagement.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

using DirectoryCategory = ClumsyBat.DataManagement.Serialization.DirectoryHandler.DirectoryCategory;
using FileType = ClumsyBat.DataManagement.Serialization.DirectoryHandler.FileType;

namespace ClumsyBat.Serialization
{
    public class SerializationHandler
    {
        public async Task<bool> Serialize<T>(T data, string filename)
        {
            return await Task.Run(() =>
            {
                BinaryFormatter bf = new BinaryFormatter();
                string filePath = DirectoryHandler.GetFullPath(filename, DirectoryCategory.Data, FileType.dat);
                bool success = true;

                try
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        bf.Serialize(stream, data);
                    }
                }
                catch
                {
                    success = false;
                }

                return success;
            });
        }

        public async Task<T> Deserialize<T>(string filename) where T : new()
        {
            return await Task.Run(() =>
            {
                BinaryFormatter bf = new BinaryFormatter();
                string filePath = DirectoryHandler.GetFullPath(filename, DirectoryCategory.Data, FileType.dat);
                try
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            stream.Position = 0;
                            T data = (T)bf.Deserialize(stream);
                            if (data == null) return new T();
                            return data;
                        }
                        catch
                        {
                            return new T();
                        }
                    }
                }
                catch
                {
                    return new T();
                }
            });
        }
    }
}
