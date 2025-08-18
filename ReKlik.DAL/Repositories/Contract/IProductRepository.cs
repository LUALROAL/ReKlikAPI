using ReKlik.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DAL.Repositories.Contract
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByMaterialTypeAsync(string materialType);
        Task<IEnumerable<Product>> GetByCompanyAsync(int companyId);
    }
}
