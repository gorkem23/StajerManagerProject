using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;

namespace StajerManager.Controllers
{
    public class UniversiteModelsController : Controller
    {
        private readonly Context _context;

        public UniversiteModelsController(Context context)
        {
            _context = context;
        }

        // GET: UniversiteModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Universiteler.ToListAsync());
        }

        // GET: UniversiteModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universiteModel = await _context.Universiteler
                .FirstOrDefaultAsync(m => m.UniversiteID == id);
            if (universiteModel == null)
            {
                return NotFound();
            }

            return View(universiteModel);
        }

        // GET: UniversiteModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UniversiteModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UniversiteID,UniversiteAdi,Adres,Telefon,Website,Sehir,PostaKodu,OlusturmaTarihi,Aktif")] UniversiteModel universiteModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(universiteModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(universiteModel);
        }

        // GET: UniversiteModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universiteModel = await _context.Universiteler.FindAsync(id);
            if (universiteModel == null)
            {
                return NotFound();
            }
            return View(universiteModel);
        }

        // POST: UniversiteModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UniversiteID,UniversiteAdi,Adres,Telefon,Website,Sehir,PostaKodu,OlusturmaTarihi,Aktif")] UniversiteModel universiteModel)
        {
            if (id != universiteModel.UniversiteID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(universiteModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversiteModelExists(universiteModel.UniversiteID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(universiteModel);
        }

        // GET: UniversiteModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universiteModel = await _context.Universiteler
                .FirstOrDefaultAsync(m => m.UniversiteID == id);
            if (universiteModel == null)
            {
                return NotFound();
            }

            return View(universiteModel);
        }

        // POST: UniversiteModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var universiteModel = await _context.Universiteler.FindAsync(id);
            if (universiteModel != null)
            {
                _context.Universiteler.Remove(universiteModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UniversiteModelExists(int id)
        {
            return _context.Universiteler.Any(e => e.UniversiteID == id);
        }

        [HttpGet]
        [Route("UniversiteModels/GetAllUniversities")]
        public async Task<IActionResult> GetAllUniversities()
        {
            var universiteler = await _context.Universiteler
                .Select(u => new {
                    u.UniversiteID,
                    u.UniversiteAdi,
                    u.Aktif
                })
                .ToListAsync();
            return Json(universiteler);
        }
    }
}
