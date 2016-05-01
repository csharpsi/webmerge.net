using System.Threading.Tasks;
using WebMerge.Client.Enums;
using WebMerge.Client.ResponseModels;

namespace WebMerge.Client
{
    public interface IDocumentCreator
    {
        Task<Document> FromHtml(string html);
        Task<Document> FromFile(string fullyQualifiedPathToFile, DocumentType documentType);
        Task<Document> FromFile(byte[] fileBytes, DocumentType documentType);
    }
}