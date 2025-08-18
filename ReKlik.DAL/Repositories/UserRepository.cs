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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ReKlikDbContext _context;

        public UserRepository(ReKlikDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByTypeAsync(string userType)
        {
            return await _context.Users
                .Where(u => u.UserType == userType)
                .ToListAsync();
        }
    }
}
