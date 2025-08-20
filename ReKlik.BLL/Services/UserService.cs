using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ReKlik.BLL.Services.Contract;
using ReKlik.DAL.Repositories.Contract;
using ReKlik.DTO.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public UserService(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
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
        public async Task<UserDTO> UpdateCurrentUserAsync(UserUpdateDTO userDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Crear un scope completamente nuevo para esta operación
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedUserRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var scopedMapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var existingUser = await scopedUserRepository.GetByIdAsync(id);
                if (existingUser == null)
                    throw new KeyNotFoundException("Usuario no encontrado");

                // VALIDACIÓN: Verificar si el email ya existe para otro usuario
                if (!string.IsNullOrEmpty(userDto.Email) && userDto.Email != existingUser.Email)
                {
                    var userWithSameEmail = await scopedUserRepository.GetByEmailAsync(userDto.Email);
                    if (userWithSameEmail != null && userWithSameEmail.Id != id)
                    {
                        throw new InvalidOperationException($"Ya existe un usuario registrado con el email: {userDto.Email}");
                    }
                }

                // Actualizar propiedades
                existingUser.Name = userDto.Name ?? existingUser.Name;
                existingUser.Phone = userDto.Phone ?? existingUser.Phone;
                existingUser.Email = userDto.Email ?? existingUser.Email;
                // Mapear otras propiedades según sea necesario

                if (!string.IsNullOrEmpty(userDto.UserType))
                {
                    existingUser.UserType = userDto.UserType;
                }


                existingUser.UpdatedAt = DateTime.UtcNow;

                await scopedUserRepository.UpdateAsync(existingUser);
                await scopedUserRepository.SaveAsync();

                return scopedMapper.Map<UserDTO>(existingUser);
            }
        }
        public async Task<UserDTO> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Usar una nueva instancia del repositorio para evitar problemas de concurrencia
            using (var scope = _serviceProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(id);

                if (user == null)
                    throw new KeyNotFoundException("Usuario no encontrado");

                return _mapper.Map<UserDTO>(user);
            }
        }

    }
}
