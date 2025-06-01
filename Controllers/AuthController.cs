using BlazorOfficina.Data;
using BlazorOfficina.Data.Models;
using BlazorOfficina.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorOfficina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AuthController(
            IUserService userService,
            ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _userService = userService;
            _context = context;
            _emailSender = emailSender;
        }

        // --------------------------------------------------
        // 1) Registrazione
        // --------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.RegisterAsync(
                dto.Username, dto.Email, dto.Password, dto.Role);

            if (!success)
                return Conflict(new { error = "Username o Email già in uso" });

            return Ok();
        }

        // --------------------------------------------------
        // 2) Login
        // --------------------------------------------------
        [HttpPost("login")]
        [Consumes("application/x-www-form-urlencoded")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return Redirect($"/login?error=invalid&returnUrl={dto.ReturnUrl ?? ""}");

            // 1) Provo a loggare per username
            var user = await _userService.AuthenticateAsync(dto.Identifier, dto.Password);

            // 2) Se non esiste, cerco per email
            if (user == null)
            {
                var byEmail = await _userService.FindByEmailAsync(dto.Identifier);
                if (byEmail != null)
                    user = await _userService.AuthenticateAsync(byEmail.Username, dto.Password);
            }

            if (user == null)
                return Redirect($"/login?error=credentials&returnUrl={dto.ReturnUrl ?? ""}");

            // 3) Genero i claim e firmo il cookie
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.Username),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Role,           user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false });

            var dest = !string.IsNullOrEmpty(dto.ReturnUrl)
            ? dto.ReturnUrl!
            : user.Role switch
            {
            "Admin" => "/admin",
            "Customer" => "/customer",
            "Mechanic" => "/mechanic",
             _         => "/"
            };
            return Redirect(dest);

        }

        // --------------------------------------------------
        // 3) Logout
        // --------------------------------------------------
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromForm] string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var dest = !string.IsNullOrEmpty(returnUrl) ? returnUrl! : "/";
            return Redirect(dest);
        }

        // --------------------------------------------------
        // 4) Richiesta reset password
        // --------------------------------------------------
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // genera e salva token
                var token = Guid.NewGuid().ToString();
                var record = new PasswordResetToken
                {
                    UserId = user.Id,
                    Token = token,
                    Expiry = DateTime.UtcNow.AddHours(1)
                };
                _context.PasswordResetTokens.Add(record);
                await _context.SaveChangesAsync();

                // invia email
                var link = $"{Request.Scheme}://{Request.Host}/reset-password?token={token}";
                var body = $"Ciao {user.Username},<br/>" +
                           $"clicca <a href=\"{link}\">qui</a> per resettare la password. " +
                           "Il link scade in 1 ora.";
                await _emailSender.SendEmailAsync(user.Email, "Reset password", body);
            }

            // messaggio generico (non rivelare se esiste o no)
            return Ok(new { Message = "Se l’indirizzo esiste, hai ricevuto un link per resettare la password." });
        }

        // --------------------------------------------------
        // 5) Esecuzione reset password
        // --------------------------------------------------
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var record = await _context.PasswordResetTokens
                                       .SingleOrDefaultAsync(t => t.Token == model.Token);
            if (record == null || record.Expiry < DateTime.UtcNow)
                return BadRequest(new { error = "Token non valido o scaduto." });

            var user = await _userService.FindByIdAsync(record.UserId);
            if (user == null)
                return BadRequest(new { error = "Utente non trovato." });

            // aggiorna password
            user.PasswordHash = _userService.Hash(model.NewPassword);
            await _userService.UpdateAsync(user);

            // elimina token
            _context.PasswordResetTokens.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password aggiornata con successo." });
        }

        // --------------------------------------------------
        // DTO classes
        // --------------------------------------------------
        public class RegisterDto
        {
            [Required] public string Username { get; set; } = string.Empty;
            [Required, EmailAddress] public string Email { get; set; } = string.Empty;
            [Required] public string Password { get; set; } = string.Empty;
            [Required] public string Role { get; set; } = string.Empty;
        }

        public class LoginDto
        {
            [Required] public string Identifier { get; set; } = string.Empty;
            [Required] public string Password { get; set; } = string.Empty;
            public string? ReturnUrl { get; set; }
        }

        public class ForgotPasswordDto
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public class ResetPasswordDto
        {
            [Required]
            public string Token { get; set; } = string.Empty;
            [Required]
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}
