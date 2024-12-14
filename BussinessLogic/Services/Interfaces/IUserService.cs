using DataAccess.Entities;
using BusinessLogic.DTOs.User;
using BusinessLogic.Utils.Implementation;
using BusinessLogic.DTOs;

namespace BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task AssignRoleAsync(int userId, string roleName);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<IEnumerable<ReadUserDto>> GetAllUsersAsync();
        Task<PaginatedList<ReadUserDto>> GetAllUsersPaginatedAsync(QueryParameters queryParameters);
        Task<string> BanUserAsync(int userId);
        Task<string> UnbanUserAsync(int userId);
    }
}
