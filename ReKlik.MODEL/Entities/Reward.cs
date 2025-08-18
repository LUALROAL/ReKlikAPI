using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class Reward
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PointsEarned { get; set; }

    public string Reason { get; set; } = null!;

    public int? ScanLogId { get; set; }

    public DateTime? AwardedAt { get; set; }

    public virtual ScanLog? ScanLog { get; set; }

    public virtual User User { get; set; } = null!;
}
