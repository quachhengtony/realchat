using System;
using System.Collections.Generic;
using Realchat.Domain.Common;

namespace Realchat.Domain.Entities;

public partial class InformationChunk : BaseEntity
{
    // public string Id { get; set; } = null!;

    public DateTime CreatedTime { get; set; }

    public string Content { get; set; } = null!;

    public int ChunkNumber { get; set; }

    // public string KnowledgeBaseId { get; set; } = null!;
    public Guid KnowledgeBaseId { get; set; }

    public virtual KnowledgeBase KnowledgeBase { get; set; } = null!;
}
