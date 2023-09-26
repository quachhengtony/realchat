using System;
using System.Collections.Generic;
using Realchat.Domain.Common;

namespace Realchat.Domain.Entities;

public partial class Script : BaseEntity
{
    // public string Id { get; set; } = null!;

    public string TriggerText { get; set; } = null!;

    public string Action { get; set; } = null!;

    // public string ChatbotId { get; set; } = null!;
    public Guid ChatbotId { get; set; }

    // public string OrganizationId { get; set; } = null!;
    public Guid OrganizationId { get; set; }

    public int ScriptTypeId { get; set; }

    public DateTime? CreatedTime { get; set; }

    public virtual Chatbot Chatbot { get; set; } = null!;

    public virtual Organization Organization { get; set; } = null!;

    public virtual ScriptType ScriptType { get; set; } = null!;
}
