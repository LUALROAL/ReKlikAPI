using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class VwProductTraceability
{
    public Guid QrCode { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Brand { get; set; }

    public string MaterialType { get; set; } = null!;

    public string Company { get; set; } = null!;

    public int? ScanCount { get; set; }

    public DateTime? FirstScan { get; set; }

    public DateTime? LastScan { get; set; }
}
