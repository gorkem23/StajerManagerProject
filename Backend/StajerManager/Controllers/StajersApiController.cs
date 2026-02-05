using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;

namespace StajerManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StajersApiController : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<StajersApiController> _logger;

        public StajersApiController(Context context, ILogger<StajersApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Stajers
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStajers(
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] string? searchText,
            [FromQuery] int? departmanID,
            [FromQuery] int? universiteID,
            [FromQuery] int? projeID,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 20;
                if (pageSize > 1000) pageSize = 1000;
                
                _logger.LogInformation("GetStajers called - page: {Page}, pageSize: {PageSize}, sortBy: {SortBy}, sortOrder: {SortOrder}", 
                    page, pageSize, sortBy, sortOrder);

                //var skip = (page - 1) * pageSize;

                var query = _context.Stajers
                    .Include(s => s.Departman)
                    .Include(s => s.Universite)
                    .Include(s => s.Bolum)
                    .Include(s => s.StajerProjeler)
                    .ThenInclude(sp => sp.Proje)
                    .AsNoTracking() // Read-only sorgular için performans iyileştirmesi
                    .AsQueryable();

            sortBy ??= "StajerID";
            sortOrder = (sortOrder?.ToLower() == "asc") ? "asc" : "desc";

            // Filtreleme parametreleri - Debug log
            _logger.LogInformation("GetStajers called with filters - departmanID: {DepartmanID}, universiteID: {UniversiteID}, projeID: {ProjeID}", 
                departmanID, universiteID, projeID);

            // Filtreleme parametreleri
            if (departmanID.HasValue)
            {
                query = query.Where(s => s.DepartmanID == departmanID.Value);
                _logger.LogInformation("Applied departmanID filter: {DepartmanID}", departmanID.Value);
            }

            if (universiteID.HasValue)
            {
                query = query.Where(s => s.UniversiteID != null && s.UniversiteID == universiteID.Value);
                _logger.LogInformation("Applied universiteID filter: {UniversiteID}", universiteID.Value);
            }

            if (projeID.HasValue)
            {
                query = query.Where(s => s.StajerProjeler.Any(sp => sp.ProjeID == projeID.Value));
                _logger.LogInformation("Applied projeID filter: {ProjeID}", projeID.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var lower = searchText.Trim().ToLower();
                query = query.Where(s =>
                    (!string.IsNullOrEmpty(s.FullName) && s.FullName.ToLower().Contains(lower)) ||
                    (!string.IsNullOrEmpty(s.Email) && s.Email.ToLower().Contains(lower)) ||
                    (!string.IsNullOrEmpty(s.PhoneNumber) && s.PhoneNumber.Contains(lower)) ||
                    (s.Universite != null && !string.IsNullOrEmpty(s.Universite.UniversiteAdi) && s.Universite.UniversiteAdi.ToLower().Contains(lower)) ||
                    (s.Bolum != null && !string.IsNullOrEmpty(s.Bolum.BolumAdi) && s.Bolum.BolumAdi.ToLower().Contains(lower)) ||
                    (s.Departman != null && !string.IsNullOrEmpty(s.Departman.DepartmanAdi) && s.Departman.DepartmanAdi.ToLower().Contains(lower))
                );
            }

            var total = await query.CountAsync();

            query = (sortBy, sortOrder) switch
            {
                ("FullName", "asc") => query.OrderBy(s => s.FullName),
                ("FullName", "desc") => query.OrderByDescending(s => s.FullName),
                ("Email", "asc") => query.OrderBy(s => s.Email),
                ("Email", "desc") => query.OrderByDescending(s => s.Email),
                ("PhoneNumber", "asc") => query.OrderBy(s => s.PhoneNumber),
                ("PhoneNumber", "desc") => query.OrderByDescending(s => s.PhoneNumber),
                ("Universite", "asc") => query.OrderBy(s => s.Universite != null ? s.Universite.UniversiteAdi : string.Empty),
                ("Universite", "desc") => query.OrderByDescending(s => s.Universite != null ? s.Universite.UniversiteAdi : string.Empty),
                ("Bolum", "asc") => query.OrderBy(s => s.Bolum != null ? s.Bolum.BolumAdi : string.Empty),
                ("Bolum", "desc") => query.OrderByDescending(s => s.Bolum != null ? s.Bolum.BolumAdi : string.Empty),
                ("Departman", "asc") => query.OrderBy(s => s.Departman != null ? s.Departman.DepartmanAdi : string.Empty),
                ("Departman", "desc") => query.OrderByDescending(s => s.Departman != null ? s.Departman.DepartmanAdi : string.Empty),
                ("StartDate", "asc") => query.OrderBy(s => s.StartDate),
                ("StartDate", "desc") => query.OrderByDescending(s => s.StartDate),
                ("EndDate", "asc") => query.OrderBy(s => s.EndDate),
                ("EndDate", "desc") => query.OrderByDescending(s => s.EndDate),
                ("StajerID", "asc") => query.OrderBy(s => s.StajerID),
                _ => query.OrderByDescending(s => s.StajerID)
            };

            var stajers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                var response = stajers.Select(MapStajerListItem).ToList();

                return Ok(new
                {
                    success = true,
                    items = response,
                    total = total, 
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize),
                    sortBy,
                    sortOrder,
                    searchText = searchText ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetStajers endpoint'inde hata oluştu: {Message}, StackTrace: {StackTrace}", 
                    ex.Message, ex.StackTrace);
                return StatusCode(500, new { 
                    success = false, 
                    message = "Stajerler yüklenirken bir hata oluştu: " + ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // GET: api/Stajers/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var stajer = await _context.Stajers
                .Include(s => s.Departman)
                .Include(s => s.Universite)
                .Include(s => s.Bolum)
                .Include(s => s.StajerProjeler)
                    .ThenInclude(sp => sp.Proje)
                .FirstOrDefaultAsync(s => s.StajerID == id);

            if (stajer == null)
            {
                return NotFound(new { success = false, message = "Stajer bulunamadı" });
            }

            return Ok(new { success = true, data = MapStajerDetail(stajer) });
        }

        // GET: api/Stajers/bolumler/{universiteId}
        [HttpGet("bolumler/{universiteId:int}")]
        //[Authorize]
        public async Task<IActionResult> GetBolumlerByUniversite(int universiteId)
        {
            var bolumler = await _context.Bolumler
                .Where(b => b.UniversiteID == universiteId)
                .OrderBy(b => b.BolumAdi)
                .Select(b => new
                {
                    bolumID = b.BolumID,
                    bolumAdi = b.BolumAdi,
                    universiteID = b.UniversiteID,
                    aktif = b.Aktif
                })
                .ToListAsync();

            return Ok(new { success = true, bolumler });
        }

        // POST: api/Stajers
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] StajerRequest request)
        {
            var validationError = ValidateRequest(request);
            if (validationError != null)
            {
                return BadRequest(new { success = false, message = validationError });
            }

            try
            {
                var normalizedEmail = request.Email!.Trim().ToLower();
                var emailExists = await _context.Stajers.AnyAsync(s => s.Email.ToLower() == normalizedEmail);

                if (emailExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Bu e-posta adresi ({request.Email}) zaten sistemde kayıtlı. Lütfen farklı bir e-posta adresi kullanın."
                    });
                }

                var stajer = BuildStajerModel(request);
                _context.Stajers.Add(stajer);
                await _context.SaveChangesAsync();

                // Many-to-many proje ilişkilerini ekle
                if (request.ProjeIDs != null && request.ProjeIDs.Any())
                {
                    foreach (var projeID in request.ProjeIDs.Where(id => id > 0))
                    {
                        var projeExists = await _context.Projeler.AnyAsync(p => p.ProjeID == projeID);
                        if (projeExists)
                        {
                            var stajerProje = new StajerProjeModel
                            {
                                StajerID = stajer.StajerID,
                                ProjeID = projeID
                            };
                            _context.StajerProjeler.Add(stajerProje);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Stajer başarıyla eklendi", data = MapStajerDetail(stajer) });
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Stajer oluşturulurken veritabanı hatası oluştu");
                
                // Email unique constraint hatası kontrolü
                if (ex.InnerException != null && 
                    (ex.InnerException.Message.Contains("IX_Stajers_Email", StringComparison.OrdinalIgnoreCase) || 
                    ex.InnerException.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                    ex.InnerException.Message.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new { success = false, message = $"Bu e-posta adresi ({request.Email}) zaten sistemde kayıtlı. Lütfen farklı bir e-posta adresi kullanın." });
                }
                
                return StatusCode(500, new { success = false, message = "Stajer oluşturulurken bir veritabanı hatası oluştu" });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Stajer oluşturulurken hata oluştu");
                return StatusCode(500, new { success = false, message = "Stajer oluşturulurken bir hata oluştu" });
            }
        }

        // PUT: api/Stajers/{id}
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] StajerRequest request)
        {
            var validationError = ValidateRequest(request, isUpdate: true);
            if (validationError != null)
            {
                return BadRequest(new { success = false, message = validationError });
            }

            var stajer = await _context.Stajers.FindAsync(id);
            if (stajer == null)
            {
                return NotFound(new { success = false, message = "Stajer bulunamadı" });
            }

            try
            {
                // Email kontrolü - mevcut stajerin kendi email'ini hariç tut
                var normalizedEmail = request.Email!.Trim().ToLower();
                var emailExists = await _context.Stajers
                    .AnyAsync(s => s.Email.ToLower() == normalizedEmail && s.StajerID != id);

                if (emailExists)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Bu e-posta adresi ({request.Email}) zaten sistemde kayıtlı. Lütfen farklı bir e-posta adresi kullanın."
                    });
                }

                await UpdateStajerModel(stajer, request, _context);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Stajer başarıyla güncellendi", data = MapStajerDetail(stajer) });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Stajer güncellenirken veritabanı hatası oluştu");
                
                // Email unique constraint hatası kontrolü
                if (ex.InnerException != null && 
                    (ex.InnerException.Message.Contains("IX_Stajers_Email", StringComparison.OrdinalIgnoreCase) || 
                    ex.InnerException.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
                    ex.InnerException.Message.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase)))
                {
                    return BadRequest(new { success = false, message = $"Bu e-posta adresi ({request.Email}) zaten sistemde kayıtlı. Lütfen farklı bir e-posta adresi kullanın." });
                }
                
                return StatusCode(500, new { success = false, message = "Stajer güncellenirken bir veritabanı hatası oluştu" });
            }
        }

        // DELETE: api/Stajers/{id}
        [HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var stajer = await _context.Stajers.FindAsync(id);
            if (stajer == null)
            {
                return NotFound(new { success = false, message = "Stajer bulunamadı" });
            }

            try
            {
                _context.Stajers.Remove(stajer);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Stajer başarıyla silindi" });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Stajer silinirken veri tabanı hatası oluştu");
                return StatusCode(500, new { success = false, message = "Stajer silinirken bir hata oluştu" });
            }
        }

        private static object MapStajerListItem(StajerModel s)
        {
            try
            {
                var projeIDs = new List<int>();
                var projeler = new List<object>();

                if (s.StajerProjeler != null && s.StajerProjeler.Any())
                {
                    projeIDs = s.StajerProjeler.Select(sp => sp.ProjeID).ToList();
                    projeler = s.StajerProjeler
                        .Where(sp => sp.Proje != null)
                        .Select(sp => new { projeID = sp.ProjeID, projeAdi = sp.Proje!.ProjeAdi })
                        .ToList<object>();
                }

                return new
                {
                    stajerID = s.StajerID,
                    fullName = s.FullName ?? string.Empty,
                    email = s.Email ?? string.Empty,
                    phoneNumber = s.PhoneNumber ?? string.Empty,
                    universiteID = s.UniversiteID,
                    bolumID = s.BolumID,
                    departmanID = s.DepartmanID,
                    projeIDs = projeIDs,
                    projeler = projeler,
                    universite = s.Universite != null ? new { universiteAdi = s.Universite.UniversiteAdi ?? string.Empty } : null,
                    bolum = s.Bolum != null ? new { bolumAdi = s.Bolum.BolumAdi ?? string.Empty } : null,
                    departman = s.Departman != null ? new { departmanAdi = s.Departman.DepartmanAdi ?? string.Empty } : null,
                    startDate = s.StartDate.ToString("yyyy-MM-dd"),
                    endDate = s.EndDate.ToString("yyyy-MM-dd"),
                    notes = s.Notes ?? string.Empty
                };
            }
            catch (Exception)
            {
                // Hata durumunda minimal bilgi döndür
                return new
                {
                    stajerID = s.StajerID,
                    fullName = s.FullName ?? string.Empty,
                    email = s.Email ?? string.Empty,
                    phoneNumber = s.PhoneNumber ?? string.Empty,
                    universiteID = s.UniversiteID,
                    bolumID = s.BolumID,
                    departmanID = s.DepartmanID,
                    projeIDs = new List<int>(),
                    projeler = new List<object>(),
                    universite = (object?)null,
                    bolum = (object?)null,
                    departman = (object?)null,
                    startDate = s.StartDate.ToString("yyyy-MM-dd"),
                    endDate = s.EndDate.ToString("yyyy-MM-dd"),
                    notes = s.Notes ?? string.Empty
                };
            }
        }

        private static object MapStajerDetail(StajerModel s)
        {
            try
            {
                var projeIDs = new List<int>();
                var projeler = new List<object>();

                if (s.StajerProjeler != null && s.StajerProjeler.Any())
                {
                    projeIDs = s.StajerProjeler.Select(sp => sp.ProjeID).ToList();
                    projeler = s.StajerProjeler
                        .Where(sp => sp.Proje != null)
                        .Select(sp => new { projeID = sp.ProjeID, projeAdi = sp.Proje!.ProjeAdi })
                        .ToList<object>();
                }

                return new
                {
                    stajerID = s.StajerID,
                    fullName = s.FullName ?? string.Empty,
                    email = s.Email ?? string.Empty,
                    phoneNumber = s.PhoneNumber ?? string.Empty,
                    universiteID = s.UniversiteID,
                    bolumID = s.BolumID,
                    departmanID = s.DepartmanID,
                    startDate = s.StartDate.ToString("yyyy-MM-dd"),
                    projeIDs = projeIDs,
                    projeler = projeler,
                    endDate = s.EndDate.ToString("yyyy-MM-dd"),
                    notes = s.Notes ?? string.Empty,
                    universite = s.Universite != null ? new { universiteAdi = s.Universite.UniversiteAdi ?? string.Empty } : null,
                    bolum = s.Bolum != null ? new { bolumAdi = s.Bolum.BolumAdi ?? string.Empty } : null,
                    departman = s.Departman != null ? new { departmanAdi = s.Departman.DepartmanAdi ?? string.Empty } : null
                };
            }
            catch (Exception)
            {
                // Hata durumunda minimal bilgi döndür
                return new
                {
                    stajerID = s.StajerID,
                    fullName = s.FullName ?? string.Empty,
                    email = s.Email ?? string.Empty,
                    phoneNumber = s.PhoneNumber ?? string.Empty,
                    universiteID = s.UniversiteID,
                    bolumID = s.BolumID,
                    departmanID = s.DepartmanID,
                    startDate = s.StartDate.ToString("yyyy-MM-dd"),
                    projeIDs = new List<int>(),
                    projeler = new List<object>(),
                    endDate = s.EndDate.ToString("yyyy-MM-dd"),
                    notes = s.Notes ?? string.Empty,
                    universite = (object?)null,
                    bolum = (object?)null,
                    departman = (object?)null
                };
            }
        }

        private static string? ValidateRequest(StajerRequest request, bool isUpdate = false)
        {
            if (request == null)
            {
                return "Geçersiz istek";
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return "Ad Soyad gereklidir";
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return "E-posta gereklidir";
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return "Telefon gereklidir";
            }

            if (!request.DepartmanID.HasValue)
            {
                return "Departman seçilmelidir";
            }

            if (!TryParseDate(request.StartDate, out _))
            {
                return "Geçerli bir başlangıç tarihi giriniz";
            }

            if (!TryParseDate(request.EndDate, out _))
            {
                return "Geçerli bir bitiş tarihi giriniz";
            }

            var start = DateOnly.Parse(request.StartDate!);
            var end = DateOnly.Parse(request.EndDate!);
            if (start > end)
            {
                return "Bitiş tarihi başlangıç tarihinden önce olamaz";
            }

            return null;
        }

        private static StajerModel BuildStajerModel(StajerRequest request)
        {
            var stajer = new StajerModel
            {
                FullName = request.FullName!.Trim(),
                Email = request.Email!.Trim(),
                PhoneNumber = request.PhoneNumber!.Trim(),
                DepartmanID = request.DepartmanID!.Value,
                UniversiteID = request.UniversiteID,
                BolumID = request.BolumID,
                StartDate = DateOnly.Parse(request.StartDate!),
                EndDate = DateOnly.Parse(request.EndDate!),
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };
            return stajer;
        }

        private async Task UpdateStajerModel(StajerModel stajer, StajerRequest request, Context context)
        {
            stajer.FullName = request.FullName!.Trim();
            stajer.Email = request.Email!.Trim();
            stajer.PhoneNumber = request.PhoneNumber!.Trim();
            stajer.DepartmanID = request.DepartmanID!.Value;
            stajer.UniversiteID = request.UniversiteID;
            stajer.BolumID = request.BolumID;
            stajer.StartDate = DateOnly.Parse(request.StartDate!);
            stajer.EndDate = DateOnly.Parse(request.EndDate!);
            stajer.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            // Many-to-many proje ilişkilerini güncelle
            if (request.ProjeIDs != null)
            {
                // Mevcut ilişkileri kaldır
                var existingRelations = context.StajerProjeler.Where(sp => sp.StajerID == stajer.StajerID).ToList();
                context.StajerProjeler.RemoveRange(existingRelations);

                // Yeni ilişkileri ekle
                foreach (var projeID in request.ProjeIDs.Where(id => id > 0))
                {
                    // Projenin var olduğunu kontrol et
                    var projeExists = await context.Projeler.AnyAsync(p => p.ProjeID == projeID);
                    if (projeExists)
                    {
                        var stajerProje = new StajerProjeModel
                        {
                            StajerID = stajer.StajerID,
                            ProjeID = projeID
                        };
                        context.StajerProjeler.Add(stajerProje);
                    }
                }
            }
        }

        private static bool TryParseDate(string? value, out DateOnly date)
        {
            if (!string.IsNullOrWhiteSpace(value) && DateOnly.TryParse(value, out date))
            {
                return true;
            }

            date = default;
            return false;
        }
    }

    public class StajerRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? UniversiteID { get; set; }
        public int? BolumID { get; set; }
        public int? DepartmanID { get; set; }
        public List<int>? ProjeIDs { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Notes { get; set; }
    }
}


