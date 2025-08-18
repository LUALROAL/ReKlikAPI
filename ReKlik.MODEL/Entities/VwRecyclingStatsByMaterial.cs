using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class VwRecyclingStatsByMaterial
{
    public string MaterialType { get; set; } = null!;

    public int? ProductsRegistered { get; set; }

    public int? TotalScans { get; set; }

    public int? ProductsRecycled { get; set; }

    public int? UniqueUsers { get; set; }
}
