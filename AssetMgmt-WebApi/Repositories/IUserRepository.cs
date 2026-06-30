using AssetMgmt_WebApi.Models;

namespace AssetMgmt_WebApi.Repositories
{
    public interface IUserRepository
    {
        Task<Login?> GetLoginByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<Login> AddUserAsync(Login login, UserRegistration registration);
    }
}
