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
    public class BolumModelsController : Controller
    {
        private readonly Context _context;

        public BolumModelsController(Context context)
        {
            _context = context;
        }

        // GET: BolumModels
        public async Task<IActionResult> Index()
        {
            var context = _context.Bolumler.Include(b => b.Universite);
            return View(await context.ToListAsync());
        }

        // GET: BolumModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bolumModel = await _context.Bolumler
                .Include(b => b.Universite)
                .FirstOrDefaultAsync(m => m.BolumID == id);
            if (bolumModel == null)
            {
                return NotFound();
            }

            return View(bolumModel);
        }

        // GET: BolumModels/Create
        public IActionResult Create()
        {
            ViewData["UniversiteID"] = new SelectList(_context.Universiteler, "UniversiteID", "UniversiteAdi");
            return View();
        }

        // POST: BolumModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BolumID,BolumAdi,BolumKodu,Aciklama,Fakulte,EgitimSuresi,EgitimTuru,UniversiteID,OlusturmaTarihi,Aktif")] BolumModel bolumModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bolumModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UniversiteID"] = new SelectList(_context.Universiteler, "UniversiteID", "UniversiteAdi", bolumModel.UniversiteID);
            return View(bolumModel);
        }

        // GET: BolumModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bolumModel = await _context.Bolumler.FindAsync(id);
            if (bolumModel == null)
            {
                return NotFound();
            }
            ViewData["UniversiteID"] = new SelectList(_context.Universiteler, "UniversiteID", "UniversiteAdi", bolumModel.UniversiteID);
            return View(bolumModel);
        }

        // POST: BolumModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BolumID,BolumAdi,BolumKodu,Aciklama,Fakulte,EgitimSuresi,EgitimTuru,UniversiteID,OlusturmaTarihi,Aktif")] BolumModel bolumModel)
        {
            if (id != bolumModel.BolumID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bolumModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BolumModelExists(bolumModel.BolumID))
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
            ViewData["UniversiteID"] = new SelectList(_context.Universiteler, "UniversiteID", "UniversiteAdi", bolumModel.UniversiteID);
            return View(bolumModel);
        }

        // GET: BolumModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bolumModel = await _context.Bolumler
                .Include(b => b.Universite)
                .FirstOrDefaultAsync(m => m.BolumID == id);
            if (bolumModel == null)
            {
                return NotFound();
            }

            return View(bolumModel);
        }

        // POST: BolumModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bolumModel = await _context.Bolumler.FindAsync(id);
            if (bolumModel != null)
            {
                _context.Bolumler.Remove(bolumModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BolumModelExists(int id)
        {
            return _context.Bolumler.Any(e => e.BolumID == id);
        }
    }
}
