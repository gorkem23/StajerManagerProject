using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;

namespace StajerManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversiteApiController : ControllerBase
    {
        private readonly Context _context;

        public UniversiteApiController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var universiteler = await _context.Universiteler
                .Where(u => u.Aktif == true) // Sadece aktif olanları getir
                .OrderBy(u => u.UniversiteAdi)
                .Select(u => new
                {
                    u.UniversiteID,
                    u.UniversiteAdi,
                    u.Aktif,
                    u.Adres,
                    u.Telefon,
                    u.Website,
                    u.Sehir,
                    u.PostaKodu,
                    u.OlusturmaTarihi
                })
                .ToListAsync();
            return Ok(universiteler);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var universite = await _context.Universiteler
                .Where(u => u.UniversiteID == id)
                .Select(u => new
                {
                    u.UniversiteID,
                    u.UniversiteAdi,
                    u.Aktif,
                    u.Adres,
                    u.Telefon,
                    u.Website,
                    u.Sehir,
                    u.PostaKodu,
                    u.OlusturmaTarihi
                })
                .SingleOrDefaultAsync();
            if (universite == null) return NotFound();
            return Ok(universite);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] UniversiteRequest request)
        {
            try
            {
                var existingUniversite = await _context.Universiteler
                    .FirstOrDefaultAsync(u => u.UniversiteAdi.ToLower() == request.UniversiteAdi!.Trim().ToLower());
                
                if (existingUniversite != null)
                {
                    return BadRequest(new { success = false, message = "Bu üniversite zaten mevcut." });
                }

                var universite = new UniversiteModel
                {
                    UniversiteAdi = request.UniversiteAdi!.Trim(),
                    Adres = string.IsNullOrWhiteSpace(request.Adres) ? null : request.Adres.Trim(),
                    Telefon = string.IsNullOrWhiteSpace(request.Telefon) ? null : request.Telefon.Trim(),
                    Website = string.IsNullOrWhiteSpace(request.Website) ? null : request.Website.Trim(),
                    Sehir = string.IsNullOrWhiteSpace(request.Sehir) ? null : request.Sehir.Trim(),
                    PostaKodu = string.IsNullOrWhiteSpace(request.PostaKodu) ? null : request.PostaKodu.Trim(),
                    OlusturmaTarihi = DateTime.Now,
                    Aktif = true
                };

                _context.Universiteler.Add(universite);  
                await _context.SaveChangesAsync();
                
                return Ok(new { success = true, message = "Üniversite başarıyla oluşturuldu.", universiteId = universite.UniversiteID });
            }catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Üniversite oluşturulurken bir hata oluştu.", error = ex.Message });
            }

        }

        public class UniversiteRequest
        {
            public string? UniversiteAdi { get; set; }
            public string? Adres { get; set; }
            public string? Telefon { get; set; }
            public string? Website { get; set; }
            public string? Sehir { get; set; }
            public string? PostaKodu { get; set; }
        }
    }
}

