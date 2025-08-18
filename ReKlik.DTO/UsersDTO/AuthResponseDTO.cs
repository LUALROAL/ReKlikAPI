using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DTO.UsersDTO
{
    public class AuthResponseDTO
    {

        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserDTO User { get; set; }

    }
}
