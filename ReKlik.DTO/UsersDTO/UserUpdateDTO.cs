using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.UsersDTO
{
    public class UserUpdateDTO
    {
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string Phone { get; set; }

        [RegularExpression("^(reciclador|ciudadano|punto_acopio|administrador)$",
            ErrorMessage = "Tipo de usuario no válido")]
        public string UserType { get; set; }
    }
}
