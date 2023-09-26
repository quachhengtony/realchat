namespace Realchat.Application.Dto;

public sealed record ChatbotResponse
{
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
    public string? OrganizationId { get; set; }
}