using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.ProductDTO
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "El ID de compañía es requerido")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "La marca no puede exceder 100 caracteres")]
        public string Brand { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El tipo de material es requerido")]
        [RegularExpression("^(PET|vidrio|carton|aluminio|papel|plastico|metal|otros)$",
            ErrorMessage = "Tipo de material no válido")]
        public string MaterialType { get; set; }

        [Range(0.01, 1000, ErrorMessage = "El peso debe estar entre 0.01 y 1000")]
        public decimal? Weight { get; set; }

        public bool Recyclable { get; set; } = true;

        [StringLength(500, ErrorMessage = "Las instrucciones no pueden exceder 500 caracteres")]
        public string RecyclingInstructions { get; set; }

        [Url(ErrorMessage = "La URL de la imagen no es válida")]
        public string ImageUrl { get; set; }
    }
}