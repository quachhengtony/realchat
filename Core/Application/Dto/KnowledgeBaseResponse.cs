namespace Realchat.Application.Dto;

public sealed record KnowledgeBaseResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid ChatbotId { get; set; }
    public Guid OrganizationId { get; set; }
}