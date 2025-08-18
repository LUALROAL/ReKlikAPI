using ReKlik.BLL.Services.Contract;
using ReKlik.DAL.Repositories.Contract;
using ReKlik.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReKlik.MODEL.Entities;


namespace ReKlik.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products.ToList());
        }

        public async Task<ProductDTO> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateAsync(ProductCreateDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();

            return _mapper.Map<ProductDTO>(product);
        }

        public async Task UpdateAsync(int id, ProductDTO productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found");

            _mapper.Map(productDto, existingProduct);
            existingProduct.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(existingProduct);
            await _productRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Product not found");

            await _productRepository.DeleteAsync(id);
            await _productRepository.SaveAsync();
        }

        public async Task<IEnumerable<ProductDTO>> GetByMaterialTypeAsync(string materialType)
        {
            var products = await _productRepository.GetByMaterialTypeAsync(materialType);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<IEnumerable<ProductDTO>> GetByCompanyAsync(int companyId)
        {
            var products = await _productRepository.GetByCompanyAsync(companyId);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }
    }
}
