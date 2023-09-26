namespace Realchat.Application.Storage;

public interface IMinioAdapter
{
    Task UploadAndPreprocessFile(string objectPath, Stream fileStream, Guid organizationId, Guid chatbotId, Guid knowledgeBaseId);
    Task ListBuckets();
}