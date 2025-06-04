using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorOfficina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeicoloController : ControllerBase
    {
        private readonly IVeicoloService _service;
        public VeicoloController(IVeicoloService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Aggiungi([FromBody] AggiungiVeicoloDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var creato = await _service.CreaVeicoloAsync(userIdClaim, dto);

            return CreatedAtAction(nameof(GetById), new { id = creato.Id }, creato);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _service.GetByIdAsync(id);
            return v is null ? NotFound() : Ok(v);
        }
    }
}
