using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace StajerManager.Controllers
{
    [Authorize] 
    public class StajersController : BaseController<StajerModel>
    {
        public StajersController(Context context) : base(context, context.Stajers)
        {
        }

        // GET: Stajers
        public async Task<IActionResult> Index(string? sortBy, string? sortOrder, string? searchText)
        {
            ViewBag.SortBy = sortBy ?? "StajerID";
            ViewBag.SortOrder = sortOrder ?? "desc";
            ViewBag.SearchText = searchText;

            var query = _context.Stajers
                .Include(s => s.Departman)
                .Include(s => s.Universite)
                .Include(s => s.Bolum)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(s => 
                    s.FullName.ToLower().Contains(searchText) ||
                    s.Email.ToLower().Contains(searchText) ||
                    s.PhoneNumber.Contains(searchText) ||
                    (s.Universite != null && s.Universite.UniversiteAdi.ToLower().Contains(searchText)) ||
                    (s.Bolum != null && s.Bolum.BolumAdi.ToLower().Contains(searchText)) ||
                    (s.Departman != null && s.Departman.DepartmanAdi.ToLower().Contains(searchText))
                );
            }

            query = (ViewBag.SortBy, ViewBag.SortOrder) switch
            {
                ("FullName", "asc") => query.OrderBy(s => s.FullName),
                ("FullName", "desc") => query.OrderByDescending(s => s.FullName),
                ("Email", "asc") => query.OrderBy(s => s.Email),
                ("Email", "desc") => query.OrderByDescending(s => s.Email),
                ("PhoneNumber", "asc") => query.OrderBy(s => s.PhoneNumber),
                ("PhoneNumber", "desc") => query.OrderByDescending(s => s.PhoneNumber),
                ("Universite", "asc") => query.OrderBy(s => s.Universite.UniversiteAdi),
                ("Universite", "desc") => query.OrderByDescending(s => s.Universite.UniversiteAdi),
                ("Bolum", "asc") => query.OrderBy(s => s.Bolum.BolumAdi),
                ("Bolum", "desc") => query.OrderByDescending(s => s.Bolum.BolumAdi),
                ("Departman", "asc") => query.OrderBy(s => s.Departman.DepartmanAdi),
                ("Departman", "desc") => query.OrderByDescending(s => s.Departman.DepartmanAdi),
                ("StartDate", "asc") => query.OrderBy(s => s.StartDate),
                ("StartDate", "desc") => query.OrderByDescending(s => s.StartDate),
                ("EndDate", "asc") => query.OrderBy(s => s.EndDate),
                ("EndDate", "desc") => query.OrderByDescending(s => s.EndDate),
                ("StajerID", "asc") => query.OrderBy(s => s.StajerID),
                ("StajerID", "desc") => query.OrderByDescending(s => s.StajerID),
                _ => query.OrderBy(s => s.FullName)
            };

            return View(await query.ToListAsync());
        }

        // GET: Stajers/GetStajers - AJAX endpoint for SPA
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStajers(string? sortBy, string? sortOrder, string? searchText)
        {
            ViewBag.SortBy = sortBy ?? "StajerID";
            ViewBag.SortOrder = sortOrder ?? "desc";
            ViewBag.SearchText = searchText;

            var query = _context.Stajers
                .Include(s => s.Departman)
                .Include(s => s.Universite)
                .Include(s => s.Bolum)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim().ToLower();
                query = query.Where(s => 
                    s.FullName.ToLower().Contains(searchText) ||
                    s.Email.ToLower().Contains(searchText) ||
                    s.PhoneNumber.Contains(searchText) ||
                    (s.Universite != null && s.Universite.UniversiteAdi.ToLower().Contains(searchText)) ||
                    (s.Bolum != null && s.Bolum.BolumAdi.ToLower().Contains(searchText)) ||
                    (s.Departman != null && s.Departman.DepartmanAdi.ToLower().Contains(searchText))
                );
            }

            query = (ViewBag.SortBy, ViewBag.SortOrder) switch
            {
                ("StajerID", "asc") => query.OrderBy(s => s.StajerID),
                ("StajerID", "desc") => query.OrderByDescending(s => s.StajerID),
                ("FullName", "asc") => query.OrderBy(s => s.FullName),
                ("FullName", "desc") => query.OrderByDescending(s => s.FullName),
                ("Email", "asc") => query.OrderBy(s => s.Email),
                ("Email", "desc") => query.OrderByDescending(s => s.Email),
                ("PhoneNumber", "asc") => query.OrderBy(s => s.PhoneNumber),
                ("PhoneNumber", "desc") => query.OrderByDescending(s => s.PhoneNumber),
                ("Universite", "asc") => query.OrderBy(s => s.Universite.UniversiteAdi),
                ("Universite", "desc") => query.OrderByDescending(s => s.Universite.UniversiteAdi),
                ("Bolum", "asc") => query.OrderBy(s => s.Bolum.BolumAdi),
                ("Bolum", "desc") => query.OrderByDescending(s => s.Bolum.BolumAdi),
                ("Departman", "asc") => query.OrderBy(s => s.Departman.DepartmanAdi),
                ("Departman", "desc") => query.OrderByDescending(s => s.Departman.DepartmanAdi),
                ("StartDate", "asc") => query.OrderBy(s => s.StartDate),
                ("StartDate", "desc") => query.OrderByDescending(s => s.StartDate),
                ("EndDate", "asc") => query.OrderBy(s => s.EndDate),
                ("EndDate", "desc") => query.OrderByDescending(s => s.EndDate),
                _ => query.OrderByDescending(s => s.StajerID)
            };

            var stajers = await query.ToListAsync();
            
            var stajersData = stajers.Select(s => new {
                stajerID = s.StajerID,
                fullName = s.FullName,
                email = s.Email,
                phoneNumber = s.PhoneNumber,
                universite = s.Universite != null ? new { universiteAdi = s.Universite.UniversiteAdi } : null,
                bolum = s.Bolum != null ? new { bolumAdi = s.Bolum.BolumAdi } : null,
                departman = s.Departman != null ? new { departmanAdi = s.Departman.DepartmanAdi } : null,
                startDate = s.StartDate,
                endDate = s.EndDate
            }).ToList();
            
            return Json(new {
                stajers = stajersData,
                totalCount = stajersData.Count,
                sortBy = ViewBag.SortBy,
                sortOrder = ViewBag.SortOrder,
                searchText = ViewBag.SearchText
            });
        }

        // GET: Stajers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var stajerModel = await _context.Stajers
                    .Include(s => s.Departman)
                    .Include(s => s.Universite)
                    .Include(s => s.Bolum)
                    .FirstOrDefaultAsync(m => m.StajerID == id);
                if (stajerModel == null) return NotFound();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_DetailsPartial", stajerModel);
                }
                return View(stajerModel);
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Detaylar yüklenirken hata oluştu: " + ex.Message });
                }
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        // GET: Stajers/GetBolumlerByUniversite/{universiteId} - AJAX endpoint
        [HttpGet]
        public async Task<IActionResult> GetBolumlerByUniversite(int universiteId)
        {
            try
            {
                var bolumler = await _context.Bolumler
                    .Where(b => b.UniversiteID == universiteId && b.Aktif == true)
                    .Select(b => new { 
                        bolumID = b.BolumID, 
                        bolumAdi = b.BolumAdi 
                    })
                    .ToListAsync();
                    
                return Json(new { 
                    success = true, 
                    bolumler = bolumler 
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "Bölümler yüklenirken hata oluştu: " + ex.Message 
                });
            }
        }

        // GET: Stajers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.DepartmanList = new SelectList(_context.Departmans, "DepartmanID", "DepartmanAdi");
            ViewBag.Universiteler = new SelectList(_context.Universiteler, "UniversiteID", "UniversiteAdi");
            // Bölümler artık JavaScript ile dinamik olarak yüklenecek
            ViewBag.Bolumler = new SelectList(new List<object>(), "BolumID", "BolumAdi");
            
            return PartialView("_CreatePartial");
        }

        // POST: Stajers/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StajerModel stajerModel)
        {
            return await CreateEntity(stajerModel, "Stajer başarıyla eklendi!");
        }

        //GET: Stajers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var stajerModel = await _context.Stajers.FindAsync(id);
                if (stajerModel == null) return NotFound();

                ViewBag.DepartmanList = new SelectList(_context.Departmans, "DepartmanID", "DepartmanAdi", stajerModel.DepartmanID);
                ViewBag.Universiteler = new SelectList(_context.Universiteler, "UniversiteID", "UniversiteAdi", stajerModel.UniversiteID);
                
                // Seçili üniversiteye göre bölümleri yükle
                var bolumler = await _context.Bolumler
                    .Where(b => b.UniversiteID == stajerModel.UniversiteID && b.Aktif == true)
                    .ToListAsync();
                ViewBag.Bolumler = new SelectList(bolumler, "BolumID", "BolumAdi", stajerModel.BolumID);
                
                return PartialView("_EditPartial", stajerModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Düzenleme formu yüklenirken hata oluştu: " + ex.Message });
            }
        }

        // POST: Stajers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [FromBody] StajerModel stajerModel)
        {
            if (id != stajerModel.StajerID)
            {
                return Json(new { success = false, message = "ID uyumsuzluğu!" });
            }

            return await UpdateEntity(id, stajerModel, "Stajer başarıyla güncellendi!");
        }

        // GET: Stajers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return NotFound();

                var stajerModel = await _context.Stajers
                    .Include(s => s.Departman)
                    .Include(s => s.Universite)
                    .Include(s => s.Bolum)
                    .FirstOrDefaultAsync(m => m.StajerID == id);
                if (stajerModel == null) return NotFound();

                return PartialView("_DeletePartial", stajerModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Silme onayı yüklenirken hata oluştu: " + ex.Message });
            }
        }

        // POST: Stajers/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return await DeleteEntity(id, "Stajer başarıyla silindi!");
        }

        private bool StajerModelExists(int id)
        {
            return _context.Stajers.Any(e => e.StajerID == id);
        }
    }
}