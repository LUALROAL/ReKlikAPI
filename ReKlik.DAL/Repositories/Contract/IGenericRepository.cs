using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace ReKlik.DAL.Repositories.Contract
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        Task<TModel> Get(Expression<Func<TModel, bool>> filter);
        Task<TModel> Create(TModel model);
        Task<bool> Update(TModel model);
        Task<bool> Delete(TModel model);
        Task<IQueryable<TModel>> GetAll(Expression<Func<TModel, bool>>? filter = null);

        // Métodos 2
        Task<IEnumerable<TModel>> GetAllAsync();           // Obtener todos
        Task<TModel> GetByIdAsync(int id);                  // Buscar por ID
        Task AddAsync(TModel model);                        // Agregar entidad
        Task SaveAsync();                                   // Guardar cambios
        Task DeleteAsync(int id);                           // Eliminar por ID
    }
}
