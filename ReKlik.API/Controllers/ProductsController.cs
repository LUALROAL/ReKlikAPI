// En ReKlik.API/Controllers/ProductsController.cs
using Microsoft.AspNetCore.Mvc;
using ReKlik.BLL.Services.Contract;
using ReKlik.DTO.ProductDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReKlik.API.Controllers
{
    [Authorize(Roles = "administrador,punto_acopio,reciclador")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            _logger.LogInformation("Obteniendo todos los productos");

            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Ocurrió un error al obtener los productos",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un producto específico por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de producto inválido: {ProductId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID del producto debe ser mayor que 0"
                });
            }

            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Producto no encontrado con ID: {ProductId}", id);
                    return NotFound(new
                    {
                        Status = "No encontrado",
                        Message = $"No se encontró un producto con ID {id}"
                    });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID: {ProductId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al obtener el producto con ID {id}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDTO>> PostProduct([FromBody] ProductCreateDTO productDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validación fallida al crear producto: {@Errors}",
                    ModelState.Values.SelectMany(v => v.Errors));

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    Status = "Error de validación",
                    Message = "Datos del producto no válidos",
                    Errors = errors
                });
            }

            try
            {
                _logger.LogInformation("Creando nuevo producto: {@ProductData}", productDto);

                var createdProduct = await _productService.CreateAsync(productDto);

                _logger.LogInformation("Producto creado exitosamente con ID: {ProductId}", createdProduct.Id);

                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, new
                {
                    Status = "Éxito",
                    Message = "Producto creado correctamente",
                    Product = createdProduct
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear producto");
                return BadRequest(new
                {
                    Status = "Error de validación",
                    Message = ex.Message,
                    Errors = ex.ValidationResult?.MemberNames
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = "Ocurrió un error al crear el producto",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductDTO productDto)
        {
            if (id != productDto.Id)
            {
                _logger.LogWarning("ID de ruta {RouteId} no coincide con ID de producto {ProductId}", id, productDto.Id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID de la ruta no coincide con el ID del producto"
                });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validación fallida al actualizar producto: {@Errors}",
                    ModelState.Values.SelectMany(v => v.Errors));

                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    Status = "Error de validación",
                    Message = "Datos del producto no válidos",
                    Errors = errors
                });
            }

            try
            {
                _logger.LogInformation("Actualizando producto con ID: {ProductId}", id);

                await _productService.UpdateAsync(id, productDto);

                _logger.LogInformation("Producto con ID {ProductId} actualizado exitosamente", id);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Producto no encontrado para actualización con ID: {ProductId}", id);
                return NotFound(new
                {
                    Status = "No encontrado",
                    Message = $"No se encontró un producto con ID {id} para actualizar"
                });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar producto con ID: {ProductId}", id);
                return BadRequest(new
                {
                    Status = "Error de validación",
                    Message = ex.Message,
                    Errors = ex.ValidationResult?.MemberNames
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto con ID: {ProductId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al actualizar el producto con ID {id}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina un producto existente
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de producto inválido para eliminación: {ProductId}", id);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID del producto debe ser mayor que 0"
                });
            }

            try
            {
                _logger.LogInformation("Eliminando producto con ID: {ProductId}", id);

                await _productService.DeleteAsync(id);

                _logger.LogInformation("Producto con ID {ProductId} eliminado exitosamente", id);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Producto no encontrado para eliminación con ID: {ProductId}", id);
                return NotFound(new
                {
                    Status = "No encontrado",
                    Message = $"No se encontró un producto con ID {id} para eliminar"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto con ID: {ProductId}", id);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al eliminar el producto con ID {id}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene productos por tipo de material
        /// </summary>
        [HttpGet("material/{materialType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByMaterial(string materialType)
        {
            var validMaterials = new[] { "PET", "vidrio", "carton", "aluminio", "papel", "plastico", "metal", "otros" };

            if (!validMaterials.Contains(materialType))
            {
                _logger.LogWarning("Tipo de material no válido: {MaterialType}", materialType);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Tipo de material no válido",
                    ValidMaterials = validMaterials
                });
            }

            try
            {
                _logger.LogInformation("Obteniendo productos por material: {MaterialType}", materialType);

                var products = await _productService.GetByMaterialTypeAsync(materialType);

                if (!products.Any())
                {
                    _logger.LogInformation("No se encontraron productos para el material: {MaterialType}", materialType);
                    return Ok(new
                    {
                        Status = "Éxito",
                        Message = $"No se encontraron productos para el material {materialType}",
                        Products = products
                    });
                }

                return Ok(new
                {
                    Status = "Éxito",
                    Message = $"Productos encontrados para el material {materialType}",
                    Count = products.Count(),
                    Products = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por material: {MaterialType}", materialType);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al obtener productos para el material {materialType}",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene productos por compañía
        /// </summary>
        [HttpGet("company/{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByCompany(int companyId)
        {
            if (companyId <= 0)
            {
                _logger.LogWarning("ID de compañía inválido: {CompanyId}", companyId);
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "El ID de la compañía debe ser mayor que 0"
                });
            }

            try
            {
                _logger.LogInformation("Obteniendo productos por compañía ID: {CompanyId}", companyId);

                var products = await _productService.GetByCompanyAsync(companyId);

                if (!products.Any())
                {
                    _logger.LogInformation("No se encontraron productos para la compañía ID: {CompanyId}", companyId);
                    return Ok(new
                    {
                        Status = "Éxito",
                        Message = $"No se encontraron productos para la compañía con ID {companyId}",
                        Products = products
                    });
                }

                return Ok(new
                {
                    Status = "Éxito",
                    Message = $"Productos encontrados para la compañía con ID {companyId}",
                    Count = products.Count(),
                    Products = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos por compañía ID: {CompanyId}", companyId);
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = $"Ocurrió un error al obtener productos para la compañía con ID {companyId}",
                    Error = ex.Message
                });
            }
        }
    }
}