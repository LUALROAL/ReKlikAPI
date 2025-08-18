using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual ICollection<ScanLog> ScanLogs { get; set; } = new List<ScanLog>();
}
