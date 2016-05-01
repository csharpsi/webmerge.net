using WebMerge.Client.Enums;

namespace WebMerge.Client.Factory
{
    public interface IDocumentCreatorFactory
    {
        IDocumentCreator Build(string name, DocumentOutputType outputType, string outputName, string folder);
    }
}