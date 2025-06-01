using BlazorOfficina.Data;
using BlazorOfficina.Data.Models;
using BlazorOfficina.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlazorOfficina.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContext = httpContextAccessor
                              ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var hash = Hash(password);
            return await _context.Users
                                 .FirstOrDefaultAsync(u => u.Username == username
                                                        && u.PasswordHash == hash);
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string role)
        {
            if (await _context.Users
                              .AnyAsync(u => u.Username == username || u.Email == email))
                return false;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = Hash(password),
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> FindByEmailAsync(string email)
            => await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        public async Task<bool> EmailExistsAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email == email);

        public async Task<User?> FindByIdAsync(int id)
            => await _context.Users
                             .AsNoTracking()
                             .FirstOrDefaultAsync(u => u.Id == id);

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var userPrincipal = _httpContext.HttpContext?.User;
            if (userPrincipal?.Identity?.IsAuthenticated != true)
                return null;

            var idClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var userId))
                return null;

            return await FindByIdAsync(userId);
        }

        public async Task<List<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _context.Users
                                 .Where(u => u.Role == roleName)
                                 .ToListAsync();
        }
    }
}
