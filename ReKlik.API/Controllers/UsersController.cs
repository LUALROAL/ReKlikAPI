using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReKlik.BLL.Services.Contract;
using ReKlik.DTO.Responses;
using ReKlik.DTO.UsersDTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReKlik.API.Controllers
{
    [Authorize(Roles = "administrador")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            _logger.LogInformation("Obteniendo todos los usuarios");

            try
            {
                var users = await _userService.GetAllUsersAsync();

                if (users == null || !users.Any())
                {
                    _logger.LogInformation("No se encontraron usuarios");
                    return Ok(new
                    {
                        Status = "Éxito",
                        Message = "No hay usuarios registrados",
                        Users = users
                    });
                }

                return Ok(new
                {
                    Status = "Éxito",
                    Count = users.Count(),
                    Users = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Ocurrió un error al obtener los usuarios",
                    Error = ex.ToString()
                });
            }
        }

        //[HttpGet]
        //[Authorize(Roles = "administrador")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<ApiResponse<IEnumerable<UserDTO>>>> GetAllUsers()
        //{
        //    try
        //    {
        //        var users = await _userService.GetAllUsersAsync();

        //        if (!users.Any())
        //        {
        //            return Ok(new ApiResponse<IEnumerable<UserDTO>>(
        //                success: true,
        //                message: "No hay usuarios registrados",
        //                data: users
        //            ));
        //        }

        //        return Ok(new ApiResponse<IEnumerable<UserDTO>>(
        //            success: true,
        //            message: "Usuarios obtenidos exitosamente",
        //            data: users
        //        ));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al obtener usuarios");
        //        return StatusCode(500, new ApiResponse<string>(
        //            success: false,
        //            message: "Error interno del servidor",
        //            error: ex.Message
        //        ));
        //    }
        //}

        /// <summary>
        /// Obtiene un usuario específico por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de usuario inválido: {UserId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID del usuario debe ser mayor que 0"
                });
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado con ID: {UserId}", id);
                    return NotFound(new
                    {
                        Status = "No encontrado",
                        Message = $"No se encontró un usuario con ID {id}"
                    });
                }

                return Ok(new
                {
                    Status = "Éxito",
                    User = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {UserId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al obtener el usuario con ID {id}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene usuarios por tipo
        /// </summary>
        [HttpGet("type/{userType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersByType(string userType)
        {
            var validUserTypes = new[] { "administrador", "punto_acopio", "reciclador", "ciudadano" };

            if (!validUserTypes.Contains(userType.ToLower()))
            {
                _logger.LogWarning("Tipo de usuario no válido: {UserType}", userType);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Tipo de usuario no válido",
                    ValidUserTypes = validUserTypes
                });
            }

            try
            {
                _logger.LogInformation("Obteniendo usuarios por tipo: {UserType}", userType);

                var users = await _userService.GetUsersByTypeAsync(userType);

                if (users == null || !users.Any())
                {
                    _logger.LogInformation("No se encontraron usuarios del tipo: {UserType}", userType);
                    return Ok(new
                    {
                        Status = "Éxito",
                        Message = $"No hay usuarios del tipo {userType}",
                        Users = users
                    });
                }

                return Ok(new
                {
                    Status = "Éxito",
                    Count = users.Count(),
                    Users = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios por tipo: {UserType}", userType);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al obtener usuarios del tipo {userType}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO userDto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de usuario inválido: {UserId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID del usuario debe ser mayor que 0"
                });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validación fallida al actualizar usuario: {@Errors}",
                    ModelState.Values.SelectMany(v => v.Errors));

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    Status = "Error de validación",
                    Message = "Datos del usuario no válidos",
                    Errors = errors
                });
            }

            try
            {
                _logger.LogInformation("Actualizando usuario con ID: {UserId}", id);

                var updatedUser = await _userService.UpdateUserAsync(id, userDto);

                _logger.LogInformation("Usuario con ID {UserId} actualizado exitosamente", id);

                return Ok(new
                {
                    Status = "Éxito",
                    Message = "Usuario actualizado correctamente",
                    User = updatedUser
                });
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Usuario no encontrado para actualización con ID: {UserId}", id);
                return NotFound(new
                {
                    Status = "No encontrado",
                    Message = $"No se encontró un usuario con ID {id} para actualizar"
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar usuario con ID: {UserId}", id);
                return BadRequest(new
                {
                    Status = "Error de validación",
                    Message = ex.Message,
                    Errors = ex.ValidationResult?.MemberNames
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID: {UserId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al actualizar el usuario con ID {id}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina un usuario existente
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de usuario inválido para eliminación: {UserId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID del usuario debe ser mayor que 0"
                });
            }

            try
            {
                _logger.LogInformation("Eliminando usuario con ID: {UserId}", id);

                await _userService.DeleteUserAsync(id);

                _logger.LogInformation("Usuario con ID {UserId} eliminado exitosamente", id);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Usuario no encontrado para eliminación con ID: {UserId}", id);
                return NotFound(new
                {
                    Status = "No encontrado",
                    Message = $"No se encontró un usuario con ID {id} para eliminar"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID: {UserId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al eliminar el usuario con ID {id}",
                    Error = ex.Message
                });
            }
        }

        // En UsersController.cs

        /// <summary>
        /// Obtiene el usuario actualmente autenticado
        /// </summary>
        [HttpGet("me")]
        [Authorize] // Requiere autenticación
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetCurrentUser()
        {
            try
            {
                var user = await _userService.GetCurrentUserAsync();
                return Ok(new
                {
                    Status = "Éxito",
                    User = user
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso no autorizado al obtener usuario actual");
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario actual");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Ocurrió un error al obtener el usuario actual",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza el usuario actualmente autenticado
        /// </summary>
        [HttpPut("UpdateCurrentUser")]
        [Authorize] // Requiere autenticación
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserUpdateDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    Status = "Error de validación",
                    Errors = errors
                });
            }

            try
            {
                var updatedUser = await _userService.UpdateCurrentUserAsync(userDto);
                return Ok(new
                {
                    Status = "Éxito",
                    Message = "Perfil actualizado correctamente",
                    User = updatedUser
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Acceso no autorizado al actualizar usuario");
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Ocurrió un error al actualizar el perfil",
                    Error = ex.Message
                });
            }
        }
    }
}