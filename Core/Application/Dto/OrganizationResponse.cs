namespace Realchat.Application.Dto;

public sealed record OrganizationResponse
{
    public Guid Id { get; set; }
    public string? DisplayName { get; set; }
}