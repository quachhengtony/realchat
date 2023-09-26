using Realchat.Domain.Common;

namespace Realchat.Domain.Entities;

public sealed class User : BaseEntity
{
    public string? Email { get; set; }
}