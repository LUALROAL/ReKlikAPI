using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.ProductDTO
{
    public class ProductDTO
    {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            public string Name { get; set; }
            public string Brand { get; set; }
            public string Description { get; set; }
            public string MaterialType { get; set; }
            public decimal? Weight { get; set; }
            public bool Recyclable { get; set; }
            public string RecyclingInstructions { get; set; }
            public string ImageUrl { get; set; }
        }
    }



