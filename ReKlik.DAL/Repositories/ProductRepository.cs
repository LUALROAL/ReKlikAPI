using ReKlik.DAL.DBContext;
using ReKlik.DAL.Repositories.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReKlik.MODEL.Entities;


namespace ReKlik.DAL.Repositories
{
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        private readonly ReKlikDbContext _context;

        public ProductRepository(ReKlikDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetByMaterialTypeAsync(string materialType)
        {
            return await _context.Products
                .Where(p => p.MaterialType == materialType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCompanyAsync(int companyId)
        {
            return await _context.Products
                .Where(p => p.CompanyId == companyId)
                .ToListAsync();
        }
    }
}
