using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WebMerge.Client.Enums;

namespace WebMerge.Client
{
    public interface IWebMergeClient
    {
        Task<byte[]> MergeDocumentAsync(int documentId, string documentKey, Dictionary<string, object> mergeDictionary, bool download = true, bool testMode = false);
        IDocumentCreator CreateDocument(string name, DocumentOutputType output = DocumentOutputType.Pdf, string outputName = null, string folder = null);
    }
}