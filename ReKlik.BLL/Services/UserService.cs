using AutoMapper;
using ReKlik.BLL.Services.Contract;
using ReKlik.DAL.Repositories.Contract;
using ReKlik.DTO.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> GetUsersByTypeAsync(string userType)
        {
            var users = await _userRepository.GetUsersByTypeAsync(userType);
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UserUpdateDTO userDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            _mapper.Map(userDto, existingUser);
            existingUser.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(existingUser);
            await _userRepository.SaveAsync();

            return _mapper.Map<UserDTO>(existingUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            await _userRepository.DeleteAsync(id);
            await _userRepository.SaveAsync();

            return true;
        }
    }
}
