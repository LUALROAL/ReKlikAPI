using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class ScanLog
{
    public int Id { get; set; }

    public int ProductCodeId { get; set; }

    public int UserId { get; set; }

    public string? ScanCity { get; set; }

    public string? ScanCountry { get; set; }

    public DateTime? ScannedAt { get; set; }

    public string ScanType { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual ProductCode ProductCode { get; set; } = null!;

    public virtual ICollection<Reward> Rewards { get; set; } = new List<Reward>();

    public virtual User User { get; set; } = null!;
}
