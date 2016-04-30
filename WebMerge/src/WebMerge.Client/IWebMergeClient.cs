using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebMerge.Client
{
    public interface IWebMergeClient
    {
        Task<byte[]> MergeDocumentAsync(int documentId, string documentKey, Dictionary<string, object> mergeDictionary, bool download = true, bool testMode = false);
    }
}