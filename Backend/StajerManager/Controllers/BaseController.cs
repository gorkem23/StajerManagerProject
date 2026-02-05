using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;

namespace StajerManager.Controllers
{
    public abstract class BaseController<T> : Controller where T : class
    {
        protected readonly Context _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseController(Context context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }

        // Ortak Create metodu
        protected virtual async Task<IActionResult> CreateEntity(T model, string successMessage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = successMessage });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = string.Join(", ", errors);
                return Json(new { success = false, message = "Validation hatası: " + errorMessage });
            }
            catch (DbUpdateException ex)
            {
                // Database constraint violations
                if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    return Json(new { success = false, message = "Bu kayıt zaten mevcut!" });
                }
                if (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    return Json(new { success = false, message = "Geçersiz referans! Lütfen seçimlerinizi kontrol edin." });
                }
                return Json(new { success = false, message = "Veritabanı hatası: " + ex.InnerException?.Message ?? ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = "İşlem hatası: " + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Beklenmeyen hata: " + ex.Message });
            }
        }

        // Ortak Update metodu
        protected virtual async Task<IActionResult> UpdateEntity(int id, T model, string successMessage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = successMessage });
                }

                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = string.Join(", ", errors);
                return Json(new { success = false, message = "Validation hatası: " + errorMessage });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, message = "Bu kayıt başka bir kullanıcı tarafından değiştirilmiş. Lütfen sayfayı yenileyin." });
            }
            catch (DbUpdateException ex)
            {
                // Database constraint violations
                if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    return Json(new { success = false, message = "Bu kayıt zaten mevcut!" });
                }
                if (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    return Json(new { success = false, message = "Geçersiz referans! Lütfen seçimlerinizi kontrol edin." });
                }
                return Json(new { success = false, message = "Veritabanı hatası: " + (ex.InnerException?.Message ?? ex.Message) });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = "İşlem hatası: " + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Beklenmeyen hata: " + ex.Message });
            }
        }

        // Ortak Delete metodu
        protected virtual async Task<IActionResult> DeleteEntity(int id, string successMessage)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = successMessage });
                }
                return Json(new { success = false, message = "Kayıt bulunamadı!" });
            }
            catch (DbUpdateException ex)
            {
                // Foreign key constraint violation
                if (ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    return Json(new { success = false, message = "Bu kayıt başka kayıtlarla ilişkili olduğu için silinemez!" });
                }
                return Json(new { success = false, message = "Veritabanı hatası: " + (ex.InnerException?.Message ?? ex.Message) });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = "İşlem hatası: " + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Beklenmeyen hata: " + ex.Message });
            }
        }
    }
}
