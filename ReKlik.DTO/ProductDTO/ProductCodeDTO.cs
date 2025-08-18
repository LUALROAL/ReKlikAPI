using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.ProductDTO
{
    public class ProductCodeDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Guid UuidCode { get; set; }

        public string? BatchNumber { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? GeneratedAt { get; set; }

        //public virtual Product Product { get; set; } = null!;

        //public virtual ICollection<ScanLog> ScanLogs { get; set; } = new List<ScanLog>();
    }
}
