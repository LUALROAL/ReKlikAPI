using System;
using System.Collections.Generic;

namespace ReKlik.MODEL.Entities;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
