using System;
using System.Collections.Generic;
using Realchat.Domain.Common;

namespace Realchat.Domain.Entities;

public partial class Organization : BaseEntity
{
    // public string Id { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public DateTime? CreatedTime { get; set; }

    public virtual ICollection<Chatbot> Chatbots { get; set; } = new List<Chatbot>();

    public virtual ICollection<KnowledgeBase> KnowledgeBases { get; set; } = new List<KnowledgeBase>();

    public virtual ICollection<Script> Scripts { get; set; } = new List<Script>();
}
