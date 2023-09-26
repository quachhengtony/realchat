namespace Realchat.Tools.Ingestor.Models;

public class CreateCollectionRequest
{
    public string collection_name { get; set; }
    public CollectionSchema schema { get; set; }
}