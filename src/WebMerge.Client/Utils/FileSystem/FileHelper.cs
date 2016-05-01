using System.IO;

namespace WebMerge.Client.Utils.FileSystem
{
    public class FileHelper : IFileHelper
    {
        public bool Exists(string path) => File.Exists(path);
        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
    }
}