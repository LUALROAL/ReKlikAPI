using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Description { get; set; }

    public string MaterialType { get; set; } = null!;

    public decimal? Weight { get; set; }

    public bool? Recyclable { get; set; }

    public string? RecyclingInstructions { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<ProductCode> ProductCodes { get; set; } = new List<ProductCode>();
}
