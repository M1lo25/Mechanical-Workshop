using BlazorOfficina.Data.Dtos;
using BlazorOfficina.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlazorOfficina.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeicoloController : ControllerBase
    {
        private readonly IVeicoloService _service;
        public VeicoloController(IVeicoloService service) => _service = service;

        [HttpPost("aggiungi")]
        public async Task<IActionResult> Aggiungi([FromBody] AggiungiVeicoloDto dto)
        {
            var creato = await _service.CreaVeicoloAsync(dto);
            // CreatedAtAction punterà al GET seguente
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
