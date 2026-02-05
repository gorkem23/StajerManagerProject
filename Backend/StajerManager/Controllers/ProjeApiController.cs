using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;
using Microsoft.AspNetCore.Http;
using StajerManager.Services;

namespace StajerManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjeApiController : ControllerBase
    {
        private readonly Context _context;
        private readonly IAzureBlobStorageService? _blobStorageService;
        private readonly ILogger<ProjeApiController> _logger;

        public ProjeApiController(
            Context context,
            ILogger<ProjeApiController> logger,
            IAzureBlobStorageService? blobStorageService = null)
        {
            _context = context;
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        [HttpGet("test-storage")]
        [Authorize]
        public IActionResult TestStorage()
        {
            if (_blobStorageService == null)
            {
                return BadRequest(new { success = false, message = "Azure Blob Storage servisi null (DI'da kayıtlı değil)." });
            }
            
            // Servis var ama yapılandırılmamış olabilir
            try
            {
                // Servisin yapılandırılıp yapılandırılmadığını test etmek için
                // GetType() kullanarak servis tipini kontrol edelim
                var serviceType = _blobStorageService.GetType().Name;
                return Ok(new { 
                    success = true, 
                    message = $"Azure Blob Storage servisi mevcut. Tip: {serviceType}",
                    serviceConfigured = _blobStorageService != null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Servis test hatası: {ex.Message}" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var projeler = await _context.Projeler
                    .OrderByDescending(p => p.BitisTarihi ?? DateTime.MaxValue)
                    .ThenByDescending(p => p.BaslangicTarihi)
                    .Select(p => new
                    {
                        p.ProjeID,
                        p.ProjeAdi,
                        p.Aciklama,
                        p.BaslangicTarihi,
                        p.BitisTarihi,
                        p.Aktif,
                        p.DosyaYolu,
                        p.DosyaAdi
                    })
                    .ToListAsync();
                return Ok(projeler);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Projeler listelenirken hata oluştu: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = "Projeler yüklenirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("{id:int}/{includeInactive:bool}")]
        [Authorize]
        public async Task<IActionResult> GetStajersByProje(int id, bool includeInactive)
        {
            try
            {
                var proje = await _context.Projeler.FindAsync(id);
                if (proje == null)
                {
                    return NotFound(new { success = false, message = "Proje bulunamadı." });
                }

                var query = _context.Stajers
                    .Include(s => s.Departman)
                    .Include(s => s.Universite)
                    .Include(s => s.Bolum)
                    .Include(s => s.StajerProjeler)
                        .ThenInclude(sp => sp.Proje)
                    .Where(s => s.StajerProjeler.Any(sp => sp.ProjeID == id));

                // Eğer includeInactive false ise, sadece aktif stajerleri getir
                if (!includeInactive)
                {
                    var today = DateOnly.FromDateTime(DateTime.Now);
                    query = query.Where(s => s.StartDate <= today && s.EndDate >= today);
                }

                var stajers = await query
                    .OrderBy(s => s.FullName)
                    .Select(s => new
                    {
                        stajerID = s.StajerID,
                        fullName = s.FullName,
                        email = s.Email,
                        phoneNumber = s.PhoneNumber,
                        universiteID = s.UniversiteID,
                        bolumID = s.BolumID,
                        departmanID = s.DepartmanID,
                        projeIDs = s.StajerProjeler.Select(sp => sp.ProjeID).ToList(),
                        projeler = s.StajerProjeler.Select(sp => new { projeID = sp.ProjeID, projeAdi = sp.Proje.ProjeAdi }).ToList(),
                        universite = s.Universite != null ? new { universiteAdi = s.Universite.UniversiteAdi } : null,
                        bolum = s.Bolum != null ? new { bolumAdi = s.Bolum.BolumAdi } : null,
                        departman = s.Departman != null ? new { departmanAdi = s.Departman.DepartmanAdi } : null,
                        startDate = s.StartDate,
                        endDate = s.EndDate,
                        notes = s.Notes
                    })
                    .ToListAsync();

                return Ok(new { success = true, stajers = stajers, total = stajers.Count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Stajerler yüklenirken bir hata oluştu." });
            }
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id){
            try{
                var proje = await _context.Projeler
                    .Where(p => p.ProjeID == id)
                    .Select(p => new
                    {
                        p.ProjeID,
                        p.ProjeAdi,
                        p.Aciklama,
                        p.BaslangicTarihi,
                        p.BitisTarihi,
                        p.Aktif,
                        p.DosyaYolu,
                        p.DosyaAdi
                    })
                    .FirstOrDefaultAsync();
                if (proje == null){
                    return NotFound(new { success = false, message = "Proje bulunamadı." });
                }
                return Ok(proje);
            }
            catch(Exception ex){
                return BadRequest(new { success = false, message = "Proje bulunurken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpGet("DownloadFile/{projeId:int}")]
        [Authorize]
        public async Task<IActionResult> DownloadFile(int projeId)
        {
            try
            {
                var proje = await _context.Projeler.FindAsync(projeId);
                if (proje == null || string.IsNullOrEmpty(proje.DosyaYolu))
                {
                    return NotFound(new { success = false, message = "Proje veya dosya bulunamadı." });
                }

                try
                {
                    if (_blobStorageService == null)
                    {
                        return BadRequest(new { success = false, message = "Azure Blob Storage servisi yapılandırılmamış." });
                    }

                    // Azure Blob Storage'dan indir
                    var fileStream = await _blobStorageService.DownloadFileAsync(proje.DosyaYolu, "projeler");
                    
                    var extension = Path.GetExtension(proje.DosyaYolu).ToLowerInvariant();
                    var contentType = extension switch
                    {
                        ".pdf" => "application/pdf",
                        ".doc" => "application/msword",
                        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                        ".xls" => "application/vnd.ms-excel",
                        ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".ppt" => "application/vnd.ms-powerpoint",
                        ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                        _ => "application/octet-stream"
                    };

                    return File(fileStream, contentType, proje.DosyaAdi ?? Path.GetFileName(proje.DosyaYolu));
                }
                catch (FileNotFoundException)
                {
                    return NotFound(new { success = false, message = "Dosya bulunamadı." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dosya indirme hatası");
                    return BadRequest(new { success = false, message = "Dosya indirilirken bir hata oluştu: " + ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya indirme hatası");
                return BadRequest(new { success = false, message = "Dosya indirilirken bir hata oluştu: " + ex.Message });
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] ProjeRequest request, IFormFile? dosya)
        {
            try
            {
                var existingProje = await _context.Projeler
                    .FirstOrDefaultAsync(p => p.ProjeAdi.ToLower() == request.ProjeAdi!.Trim().ToLower());
                if (existingProje != null)
                {
                    return BadRequest(new { success = false, message = "Bu proje zaten mevcut." });
                }

                string? dosyaYolu = null;
                string? dosyaAdi = null;

                // Dosya yükleme işlemi
                if (dosya != null && dosya.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".sln", ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(dosya.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        return BadRequest(new { success = false, message = "Dosya uzantısı geçersiz." });
                    }

                    if (dosya.Length > 10 * 1024 * 1024)
                    {
                        return BadRequest(new { success = false, message = "Dosya boyutu 10MB'den büyük olamaz." });
                    }

                    try
                    {
                        if (_blobStorageService == null)
                        {
                            return BadRequest(new { success = false, message = "Azure Blob Storage servisi yapılandırılmamış." });
                        }

                        // Yıl/Ay bazlı klasör yapısı
                        var year = DateTime.Now.Year;
                        var month = DateTime.Now.Month.ToString("00");
                        var folderPath = $"{year}/{month}";

                        // Azure Blob Storage'a yükle
                        var blobName = await _blobStorageService.UploadFileAsync(
                            dosya, 
                            "projeler", // Container adı
                            folderPath  // Klasör yolu
                        );

                        dosyaYolu = blobName;
                        dosyaAdi = dosya.FileName;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Dosya yükleme hatası");
                        return BadRequest(new { success = false, message = "Dosya yüklenirken bir hata oluştu: " + ex.Message });
                    }
                }

                var proje = new ProjeModel
                {
                    ProjeAdi = request.ProjeAdi!.Trim(),
                    Aciklama = string.IsNullOrWhiteSpace(request.Aciklama) ? null : request.Aciklama.Trim(),
                    BaslangicTarihi = request.BaslangicTarihi ?? DateTime.Now,
                    BitisTarihi = request.BitisTarihi,
                    OlusturmaTarihi = DateTime.Now,
                    Aktif = true,
                    DosyaYolu = dosyaYolu,
                    DosyaAdi = dosyaAdi
                };

                _context.Projeler.Add(proje);
                await _context.SaveChangesAsync();

                // Stajer-Proje ilişkilerini ekle
                if (request.StajerIDs != null && request.StajerIDs.Any())
                {
                    foreach (var stajerID in request.StajerIDs.Where(id => id > 0))
                    {
                        var stajerExists = await _context.Stajers.AnyAsync(s => s.StajerID == stajerID);
                        if (stajerExists)
                        {
                            // Mevcut projeleri kontrol et
                            var stajer = await _context.Stajers
                                .Include(s => s.StajerProjeler)
                                .FirstOrDefaultAsync(s => s.StajerID == stajerID);
                            
                            if (stajer != null)
                            {
                                // Eğer zaten bu projeye bağlı değilse ekle
                                var alreadyExists = stajer.StajerProjeler?.Any(sp => sp.ProjeID == proje.ProjeID) ?? false;
                                if (!alreadyExists)
                                {
                                    var stajerProje = new StajerProjeModel
                                    {
                                        StajerID = stajerID,
                                        ProjeID = proje.ProjeID
                                    };
                                    _context.StajerProjeler.Add(stajerProje);
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Proje başarıyla oluşturuldu.", projeID = proje.ProjeID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Proje oluşturulurken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProjeRequest request){
            try{
                var proje = await _context.Projeler.FindAsync(id);
                proje.Aktif = request.Aktif.Value;
                proje.Aciklama = string.IsNullOrWhiteSpace(request.Aciklama) ? null : request.Aciklama.Trim();
                if(proje == null){
                    return NotFound(new { success = false, message = "Proje bulunamadı." });
                }
                if(request.Aciklama != null){
                    proje.Aciklama = string.IsNullOrWhiteSpace(request.Aciklama) ? null : request.Aciklama.Trim();
                }
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Proje başarıyla güncellendi." });
            }
            catch(Exception ex){
                return BadRequest(new { success = false, message = "Proje güncellenirken bir hata oluştu." });
            }
        }

        [HttpDelete("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var proje = await _context.Projeler
                    .Include(p => p.StajerProjeler)
                    .FirstOrDefaultAsync(p => p.ProjeID == id);
                if (proje == null)
                {
                    return NotFound(new { success = false, message = "Proje bulunamadı." });
                }

                // Stajer-Proje ilişkilerini kaldır
                if (proje.StajerProjeler != null && proje.StajerProjeler.Any())
                {
                    _context.StajerProjeler.RemoveRange(proje.StajerProjeler);
                    await _context.SaveChangesAsync();
                }

                // Azure Blob Storage'dan dosyayı sil
                if (!string.IsNullOrEmpty(proje.DosyaYolu) && _blobStorageService != null)
                {
                    try
                    {
                        await _blobStorageService.DeleteFileAsync(proje.DosyaYolu, "projeler");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Proje dosyası silinemedi: {proje.DosyaYolu}");
                        // Dosya silinemese bile projeyi silmeye devam et
                    }
                }

                _context.Projeler.Remove(proje);
                await _context.SaveChangesAsync();
                
                return Ok(new { success = true, message = "Proje başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Proje silme hatası");
                return BadRequest(new { success = false, message = "Proje silinirken bir hata oluştu." });
            }
        }
    
        public class ProjeRequest
        {
            public string? ProjeAdi { get; set; }
            public string? Aciklama { get; set; }
            public DateTime? BaslangicTarihi { get; set; }
            public DateTime? BitisTarihi { get; set; }
            public bool? Aktif { get; set; }
            public List<int>? StajerIDs { get; set; }
        }
    }
}

