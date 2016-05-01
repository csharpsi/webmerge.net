namespace WebMerge.Client.Utils.FileSystem
{
    public interface IFileHelper
    {
        bool Exists(string path);
        byte[] ReadAllBytes(string path);
    }
}