using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazorOfficina.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BlazorOfficina.Data;         
using BlazorOfficina.Data.Models;        

[ApiController]
[Route("api/[controller]")]
[Authorize]  // Richiede utente autenticato
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public UserController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpGet("{id}/profile-image")]
    public async Task<IActionResult> GetProfileImage(int id)
    {
        // 1. Verifica che l’utente richiedente possa vedere questa immagine
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (currentUserId != id)
            return Forbid();

        // 2. Carica solo la proprietà ProfileImage
        var userImg = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => u.ProfileImage)
            .FirstOrDefaultAsync();

        // 3. Se non esiste o è vuoto, restituisci il file di default
        if (userImg == null || userImg.Length == 0)
        {
            var defaultPath = Path.Combine(_env.WebRootPath, "images", "default-profile.png");
            if (!System.IO.File.Exists(defaultPath))
                return NotFound();

            var defaultBytes = await System.IO.File.ReadAllBytesAsync(defaultPath);
            return File(defaultBytes, "image/png");
        }

        // 4. Altrimenti restituisci i byte salvati
        return File(userImg, "image/png");
    }
}
