using System;
using System.Collections.Generic;

namespace Realchat.Domain.Entities;

public partial class ScriptType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Script> Scripts { get; set; } = new List<Script>();
}
