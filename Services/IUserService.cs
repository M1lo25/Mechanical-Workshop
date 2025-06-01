using BlazorOfficina.Data.Models;
using System.Threading.Tasks;

namespace BlazorOfficina.Services
{
    /// <summary>
    /// Interfaccia per la gestione utenti (autenticazione, registrazione, reset password, ecc.)
    /// </summary>
    public interface IUserService
    {
        Task<User?> GetCurrentUserAsync();
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password, string role);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> FindByIdAsync(int id);
        Task UpdateAsync(User user);

        Task<List<User>> GetUsersByRoleAsync(string roleName);
        string Hash(string password);
    }
}
