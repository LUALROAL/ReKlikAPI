using ReKlik.DTO.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.BLL.Services.Contract
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IEnumerable<UserDTO>> GetUsersByTypeAsync(string userType);
        Task<UserDTO> UpdateUserAsync(int id, UserUpdateDTO userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserDTO> GetCurrentUserAsync(); // Nuevo método
        Task<UserDTO> UpdateCurrentUserAsync(UserUpdateDTO userDto); // Nuevo método
}
}
