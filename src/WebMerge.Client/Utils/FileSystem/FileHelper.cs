using System;
using System.IO;

namespace WebMerge.Client.Utils.FileSystem
{
    public class FileHelper
    {
        public static Func<string, byte[]> FileReadFunc { get; set; } = File.ReadAllBytes;
        public static Func<string, bool> ExistsFunc { get; set; } = File.Exists;

        public static byte[] ReadAllBytes(string path) => FileReadFunc.Invoke(path);
        public static bool Exists(string path) => ExistsFunc.Invoke(path);
    }
}