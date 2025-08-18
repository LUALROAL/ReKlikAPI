using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReKlik.BLL.Services;
using ReKlik.DTO;
using ReKlik.DTO.UsersDTO;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ReKlik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                _logger.LogInformation("Intento de inicio de sesión para el email: {Email}", loginDto.Email);

                var response = await _authService.Login(loginDto);

                _logger.LogInformation("Inicio de sesión exitoso para el usuario: {UserId}", response.User.Id);

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Error de validación en inicio de sesión: {Error}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Credenciales inválidas para el email: {Email}", loginDto.Email);
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar inicio de sesión");
                return StatusCode(500, new { Message = "Ocurrió un error al procesar el inicio de sesión" });
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDTO googleAuth)
        {
            try
            {
                _logger.LogInformation("Intento de login con Google");
                var response = await _authService.GoogleLogin(googleAuth);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Google login");
                return Unauthorized(new { Message = "Autenticación con Google fallida" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                _logger.LogInformation("Intento de registro para: {Email}", registerDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Validación fallida: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return BadRequest(ModelState);
                }

                var response = await _authService.Register(registerDto);

                _logger.LogInformation("Registro exitoso. UserId: {UserId}", response.User.Id);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación en registro");
                return BadRequest(new { Message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al registrar usuario");
                return StatusCode(500, new { Message = "Error al guardar en base de datos" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante el registro");
                return StatusCode(500, new
                {
                    Message = "Error interno del servidor",
                    Detail = ex.Message // Solo para desarrollo, quitar en producción
                });
            }
        }
    }
}