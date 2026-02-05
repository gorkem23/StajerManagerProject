using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StajerManager.Models;

namespace StajerManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmanApiController : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<DepartmanApiController> _logger;

        public DepartmanApiController(Context context, ILogger<DepartmanApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        //[AllowAnonymous] // Geçici olarak test için - production'da kaldırılmalı
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var isAdmin = User.IsInRole("Admin") || User.Identity?.Name?.ToLower() == "admin@stajermanager.com";

            var query = _context.Departmans.AsQueryable();

            if (!isAdmin)
            {
                query= query.Where(d => d.Stajers.Any());
            }

            var list = await query
                .OrderBy(d => d.DepartmanAdi)
                .Select(d => new
                {
                    d.DepartmanID,
                    d.DepartmanAdi,
                    d.Aciklama
                })
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var departman = await _context.Departmans
                .Where(x => x.DepartmanID == id)
                .Select(x => new
                {
                    x.DepartmanID,
                    x.DepartmanAdi,
                    x.Aciklama
                })
                .SingleOrDefaultAsync();

            if (departman == null)
            {
                return NotFound(new { success = false, message = "Departman bulunamadı" });
            }

            return Ok(new { success = true, data = departman });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] DepartmanRequest request)
        {
            var validationError = ValidateRequest(request);
            if (validationError != null)
            {
                return BadRequest(new { success = false, message = validationError });
            }

            try
            {
                var departman = new DepartmanModel
                {
                    DepartmanAdi = request.DepartmanAdi!.Trim(),
                    Aciklama = string.IsNullOrWhiteSpace(request.Aciklama) ? null : request.Aciklama.Trim()
                };

                _context.Departmans.Add(departman);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Departman başarıyla eklendi",
                    data = new
                    {
                        departman.DepartmanID,
                        departman.DepartmanAdi,
                        departman.Aciklama
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman oluşturulurken hata oluştu");
                return StatusCode(500, new { success = false, message = "Departman oluşturulurken bir hata oluştu" });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmanRequest request)
        {
            var validationError = ValidateRequest(request);
            if (validationError != null)
            {
                return BadRequest(new { success = false, message = validationError });
            }

            var departman = await _context.Departmans.FindAsync(id);
            if (departman == null)
            {
                return NotFound(new { success = false, message = "Departman bulunamadı" });
            }

            try
            {
                departman.DepartmanAdi = request.DepartmanAdi!.Trim();
                departman.Aciklama = string.IsNullOrWhiteSpace(request.Aciklama) ? null : request.Aciklama.Trim();

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Departman başarıyla güncellendi",
                    data = new
                    {
                        departman.DepartmanID,
                        departman.DepartmanAdi,
                        departman.Aciklama
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Departman güncellenirken hata oluştu");
                return StatusCode(500, new { success = false, message = "Departman güncellenirken bir hata oluştu" });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var departman = await _context.Departmans
                .Include(d => d.Stajers)
                .FirstOrDefaultAsync(d => d.DepartmanID == id);

            if (departman == null)
            {
                return NotFound(new { success = false, message = "Departman bulunamadı" });
            }

            if (departman.Stajers.Any())
            {
                return BadRequest(new { success = false, message = "Bu departmanda stajer bulunduğu için silinemez" });
            }

            try
            {
                _context.Departmans.Remove(departman);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Departman başarıyla silindi" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Departman silinirken veri tabanı hatası oluştu");
                return StatusCode(500, new { success = false, message = "Departman silinirken bir hata oluştu" });
            }
        }

        private static string? ValidateRequest(DepartmanRequest request)
        {
            if (request == null)
            {
                return "Geçersiz istek";
            }

            if (string.IsNullOrWhiteSpace(request.DepartmanAdi))
            {
                return "Departman adı gereklidir";
            }

            if (request.DepartmanAdi.Length > 50)
            {
                return "Departman adı en fazla 50 karakter olabilir";
            }

            if (!string.IsNullOrWhiteSpace(request.Aciklama) && request.Aciklama.Length > 200)
            {
                return "Açıklama en fazla 200 karakter olabilir";
            }

            return null;
        }
    }

    public class DepartmanRequest
    {
        public string? DepartmanAdi { get; set; }
        public string? Aciklama { get; set; }
    }
}

