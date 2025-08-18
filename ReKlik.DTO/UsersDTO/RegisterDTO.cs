using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.UsersDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        [RegularExpression("^(administrador|reciclador|ciudadano|punto_acopio)$",
            ErrorMessage = "Tipo de usuario no válido")]
        public string UserType { get; set; }

        public string Phone { get; set; }
    }
}
