using ReKlik.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.BLL.Services.Contract
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO> GetByIdAsync(int id);
        Task<ProductDTO> CreateAsync(ProductCreateDTO productDto);
        Task UpdateAsync(int id, ProductDTO productDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductDTO>> GetByMaterialTypeAsync(string materialType);
        Task<IEnumerable<ProductDTO>> GetByCompanyAsync(int companyId);
    }
}
