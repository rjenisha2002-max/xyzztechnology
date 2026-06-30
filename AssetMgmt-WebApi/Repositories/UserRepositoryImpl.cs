using AssetMgmt_WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetMgmt_WebApi.Repositories
{
    //inherited with iuserrepository
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly AssetMgmtContext _context;

        public UserRepositoryImpl(AssetMgmtContext context)
        {
            _context = context;
        }

        public async Task<Login?> GetLoginByUsernameAsync(string username)
        {
            return await _context.Logins
                .Include(l => l.UserRegistration)
                .FirstOrDefaultAsync(l => l.Username == username && l.IsActive);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Logins.AnyAsync(l => l.Username == username);
        }

        public async Task<Login> AddUserAsync(Login login, UserRegistration registration)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Logins.Add(login);
                await _context.SaveChangesAsync();

                registration.LId = login.LId;
                _context.UserRegistrations.Add(registration);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return login;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
