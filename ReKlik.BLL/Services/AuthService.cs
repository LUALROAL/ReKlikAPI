using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ReKlik.DAL.Repositories.Contract;
using ReKlik.DTO;
using ReKlik.DTO.UsersDTO;
using ReKlik.MODEL.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;


namespace ReKlik.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration config,
            IUserRepository userRepository,
            ILogger<AuthService> logger)
        {
            _config = config;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<AuthResponseDTO> Login(LoginDTO loginDto)
        {
            try
            {
                _logger.LogInformation("Iniciando proceso de login para {Email}", loginDto.Email);

                // Validación manual adicional
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    _logger.LogWarning("Email o contraseña vacíos");
                    throw new ValidationException("Email y contraseña son requeridos");
                }

                // Validación del modelo con mensajes detallados
                var validationContext = new ValidationContext(loginDto);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(loginDto, validationContext, validationResults, true))
                {
                    var errorMessages = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                    _logger.LogWarning("Validación fallida: {Errors}", errorMessages);
                    throw new ValidationException(errorMessages);
                }

                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado para el email: {Email}", loginDto.Email);
                    throw new UnauthorizedAccessException("Credenciales inválidas");
                }

                if (!VerifyPassword(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Contraseña incorrecta para el usuario: {UserId}", user.Id);
                    throw new UnauthorizedAccessException("Credenciales inválidas");
                }

                var token = GenerateToken(user);

                _logger.LogInformation("Login exitoso para el usuario: {UserId}", user.Id);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        UserType = user.UserType,
                        Phone = user.Phone
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el proceso de login");
                throw; // Re-lanzar para que el controller maneje
            }
        }
        public async Task<AuthResponseDTO> GoogleLogin(GoogleAuthDTO googleAuth)
        {
            try
            {
                _logger.LogInformation("Iniciando autenticación con Google");

                // 1. Validar el token de Google
                var payload = await ValidateGoogleToken(googleAuth.IdToken);

                // 2. Buscar o crear usuario
                var user = await _userRepository.GetByEmailAsync(payload.Email) ?? await CreateUserFromGoogle(payload);

                // 3. Generar nuestro JWT
                var token = GenerateToken(user);

                _logger.LogInformation("Google login exitoso para: {Email}", payload.Email);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        UserType = user.UserType,
                        Phone = user.Phone
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en Google login");
                throw;
            }
        }
        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _config["Google:ClientId"] }
                };

                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando token de Google");
                throw new UnauthorizedAccessException("Autenticación con Google fallida");
            }
        }

        private async Task<User> CreateUserFromGoogle(GoogleJsonWebSignature.Payload payload)
        {
            var user = new User
            {
                Name = payload.Name,
                Email = payload.Email,
                PasswordHash = HashPassword(Guid.NewGuid().ToString()), // Contraseña aleatoria
                UserType = "ciudadano", // Default role
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            return user;
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO registerDto)
        {
            try
            {
                _logger.LogInformation("Iniciando registro para {Email}", registerDto.Email);

                // Validación básica manual
                if (string.IsNullOrWhiteSpace(registerDto.Email) ||
                    string.IsNullOrWhiteSpace(registerDto.Password) ||
                    string.IsNullOrWhiteSpace(registerDto.UserType))
                {
                    _logger.LogWarning("Datos requeridos faltantes");
                    throw new ValidationException("Email, contraseña y tipo de usuario son requeridos");
                }

                // Validación del modelo con mensajes detallados
                var validationContext = new ValidationContext(registerDto);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(registerDto, validationContext, validationResults, true))
                {
                    var errorMessages = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                    _logger.LogWarning("Validación fallida: {Errors}", errorMessages);
                    throw new ValidationException(errorMessages);
                }

                // Validación de tipo de usuario
                var allowedUserTypes = new[] { "reciclador", "ciudadano", "punto_acopio", "administrador" };
                if (!allowedUserTypes.Contains(registerDto.UserType.ToLower()))
                {
                    _logger.LogWarning("Tipo de usuario no válido: {UserType}", registerDto.UserType);
                    throw new ValidationException($"Tipo de usuario no válido. Valores permitidos: {string.Join(", ", allowedUserTypes)}");
                }

                // Verificar si el usuario ya existe
                if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
                {
                    _logger.LogWarning("El email ya está registrado: {Email}", registerDto.Email);
                    throw new ValidationException("El email ya está registrado");
                }

                // Crear nuevo usuario
                var user = new User
                {
                    Name = registerDto.Name?.Trim(),
                    Email = registerDto.Email.ToLower().Trim(),
                    PasswordHash = HashPassword(registerDto.Password),
                    UserType = registerDto.UserType.ToLower(),
                    Phone = registerDto.Phone?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveAsync();

                var token = GenerateToken(user);

                _logger.LogInformation("Registro exitoso para el usuario: {UserId}", user.Id);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        UserType = user.UserType,
                        Phone = user.Phone
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro");
                throw; // Re-lanzar para que el controller maneje
            }
        }

        private string GenerateToken(User user)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType)
                };

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el token JWT");
                throw new Exception("Error al generar el token de autenticación");
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                {
                    return false;
                }

                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la contraseña");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("La contraseña no puede estar vacía");
                }

                return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al hashear la contraseña");
                throw new Exception("Error al procesar la contraseña");
            }
        }
    }

    public interface IAuthService
    {
        Task<AuthResponseDTO> Login(LoginDTO loginDto);
        Task<AuthResponseDTO> GoogleLogin(GoogleAuthDTO googleAuth);
        Task<AuthResponseDTO> Register(RegisterDTO registerDto);
    }
}