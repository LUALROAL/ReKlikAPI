using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class ProductCode
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Guid UuidCode { get; set; }

    public string? BatchNumber { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? GeneratedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ScanLog> ScanLogs { get; set; } = new List<ScanLog>();
}
