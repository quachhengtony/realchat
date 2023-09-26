using System;
using System.Collections.Generic;
using Realchat.Domain.Common;

namespace Realchat.Domain.Entities;

public partial class Chatbot : BaseEntity
{
    // public string Id { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public DateTime? CreatedTime { get; set; }

    // public string OrganizationId { get; set; } = null!;
    public Guid OrganizationId { get; set; }

    public virtual ICollection<KnowledgeBase> KnowledgeBases { get; set; } = new List<KnowledgeBase>();

    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<Script> Scripts { get; set; } = new List<Script>();
}
