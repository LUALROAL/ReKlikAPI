using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.UsersDTO
{
    public class GoogleAuthDTO
    {
        [Required]
        public string IdToken { get; set; }
    }
}
