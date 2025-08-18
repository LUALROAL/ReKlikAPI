using ReKlik.MODEL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReKlik.DAL.Repositories.Contract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<bool> UserExists(string email);
        Task<IEnumerable<User>> GetUsersByTypeAsync(string userType);
    }
    
    
}
