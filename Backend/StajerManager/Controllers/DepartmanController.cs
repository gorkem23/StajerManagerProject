using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace StajerManager.Controllers
{
    [Authorize]
    public class DepartmanController : BaseController<DepartmanModel>
    {
        public DepartmanController(Context context) : base(context, context.Departmans)
        {
        }

        // GET: Departman
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departmans.ToListAsync());
        }

        // GET: Departman/GetDepartmans - AJAX endpoint for SPA
        [HttpGet]
        public async Task<IActionResult> GetDepartmans()
        {
            var departmans = await _context.Departmans.ToListAsync();
            
            var departmansData = departmans.Select(d => new {
                departmanID = d.DepartmanID,
                departmanAdi = d.DepartmanAdi,
                aciklama = d.Aciklama
            }).ToList();
            
            return Json(new {
                departmans = departmansData,
                totalCount = departmansData.Count
            });
        }

        // GET: Departman/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var departmanModel = await _context.Departmans
                .Include(d => d.Stajers)
                .FirstOrDefaultAsync(m => m.DepartmanID == id);
            if (departmanModel == null) return NotFound();

            return PartialView("_DetailsPartial", departmanModel);
        }

        // GET: Departman/Create
        public IActionResult Create()
        {
            return PartialView("_CreatePartial");
        }

        // POST: Departman/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] DepartmanModel departmanModel)
        {
            return await CreateEntity(departmanModel, "Departman başarıyla eklendi!");
        }

        // GET: Departman/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var departmanModel = await _context.Departmans.FindAsync(id);
            if (departmanModel == null) return NotFound();
            
            return PartialView("_EditPartial", departmanModel);
        }

        // POST: Departman/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromBody] DepartmanModel departmanModel)
        {
            if (id != departmanModel.DepartmanID)
            {
                return Json(new { success = false, message = "ID uyumsuzluğu!" });
            }

            return await UpdateEntity(id, departmanModel, "Departman başarıyla güncellendi!");
        }

        // GET: Departman/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var departmanModel = await _context.Departmans
                .Include(d => d.Stajers)
                .FirstOrDefaultAsync(m => m.DepartmanID == id);
            if (departmanModel == null) return NotFound();

            return PartialView("_DeletePartial", departmanModel);
        }

        // POST: Departman/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var departmanModel = await _context.Departmans
                    .Include(d => d.Stajers)
                    .FirstOrDefaultAsync(d => d.DepartmanID == id);
                
                if (departmanModel == null)
                {
                    return Json(new { success = false, message = "Departman bulunamadı!" });
                }

                if (departmanModel.Stajers.Any())
                {
                    return Json(new { success = false, message = "Bu departmanda stajerler bulunuyor. Önce stajerleri başka departmana taşıyın!" });
                }

                _context.Departmans.Remove(departmanModel);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Departman başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }

        private bool DepartmanModelExists(int id)
        {
            return _context.Departmans.Any(e => e.DepartmanID == id);
        }
    }
}