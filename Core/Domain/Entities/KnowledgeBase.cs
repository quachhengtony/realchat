using System;
using System.Collections.Generic;
using Realchat.Domain.Common;

namespace Realchat.Domain.Entities;

public partial class KnowledgeBase : BaseEntity
{
    // public string Id { get; set; } = null!;

    public DateTime CreatedTime { get; set; }

    public string Name { get; set; } = null!;

    // public string ChatbotId { get; set; } = null!;
    public Guid ChatbotId { get; set; }

    // public string OrganizationId { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public virtual Chatbot Chatbot { get; set; } = null!;

    public virtual ICollection<InformationChunk> InformationChunks { get; set; } = new List<InformationChunk>();

    public virtual Organization Organization { get; set; } = null!;
}
