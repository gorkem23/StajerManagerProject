using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajerManager.Models;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace StajerManager.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly Context _context;

        public DashboardController(Context context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var dashboardData = await GetDashboardData();
            return View(dashboardData);
        }

        // Dashboard verilerini getir (AJAX endpoint)
        [HttpGet]
        public async Task<IActionResult> GetDashboardDataAjax()
        {
            try
            {
                var data = await GetDashboardData();
                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Departman dağılımı verilerini getir
        [HttpGet]
        public async Task<IActionResult> GetDepartmanDistribution()
        {
            try
            {
                var departmanData = await _context.Stajers
                    .Include(s => s.Departman)
                    .GroupBy(s => s.Departman.DepartmanAdi)
                    .Select(g => new
                    {
                        departman = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                return Json(new { success = true, data = departmanData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Üniversite dağılımı verilerini getir
        [HttpGet]
        public async Task<IActionResult> GetUniversiteDistribution()
        {
            try
            {
                var universiteData = await _context.Stajers
                    .Include(s => s.Universite)
                    .Where(s => s.Universite != null)
                    .GroupBy(s => s.Universite.UniversiteAdi)
                    .Select(g => new
                    {
                        universite = g.Key,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToListAsync();

                return Json(new { success = true, data = universiteData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Aylık stajer sayıları
        [HttpGet]
        public async Task<IActionResult> GetMonthlyStajerCounts()
        {
            try
            {
                var monthlyData = await _context.Stajers
                    .GroupBy(s => new { s.StartDate.Year, s.StartDate.Month })
                    .Select(g => new
                    {
                        year = g.Key.Year,
                        month = g.Key.Month,
                        monthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        count = g.Count()
                    })
                    .OrderBy(x => x.year)
                    .ThenBy(x => x.month)
                    .ToListAsync();

                return Json(new { success = true, data = monthlyData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Excel Export
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? startDate, string? endDate)
        {
            try
            {
                var query = _context.Stajers
                    .Include(s => s.Departman)
                    .Include(s => s.Universite)
                    .Include(s => s.Bolum)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                {
                    query = query.Where(s => s.StartDate >= DateOnly.FromDateTime(start));
                }

                if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                {
                    query = query.Where(s => s.StartDate <= DateOnly.FromDateTime(end));
                }

                var stajers = await query.ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Stajerler");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Ad Soyad";
                worksheet.Cell(1, 3).Value = "E-posta";
                worksheet.Cell(1, 4).Value = "Telefon";
                worksheet.Cell(1, 5).Value = "Üniversite";
                worksheet.Cell(1, 6).Value = "Bölüm";
                worksheet.Cell(1, 7).Value = "Departman";
                worksheet.Cell(1, 8).Value = "Başlangıç Tarihi";
                worksheet.Cell(1, 9).Value = "Bitiş Tarihi";
                worksheet.Cell(1, 10).Value = "Notlar";

                for (int i = 0; i < stajers.Count; i++)
                {
                    var stajer = stajers[i];
                    var row = i + 2;

                    worksheet.Cell(row, 1).Value = stajer.StajerID;
                    worksheet.Cell(row, 2).Value = stajer.FullName;
                    worksheet.Cell(row, 3).Value = stajer.Email;
                    worksheet.Cell(row, 4).Value = stajer.PhoneNumber;
                    worksheet.Cell(row, 5).Value = stajer.Universite?.UniversiteAdi ?? "";
                    worksheet.Cell(row, 6).Value = stajer.Bolum?.BolumAdi ?? "";
                    worksheet.Cell(row, 7).Value = stajer.Departman?.DepartmanAdi ?? "";
                    worksheet.Cell(row, 8).Value = stajer.StartDate.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 9).Value = stajer.EndDate.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 10).Value = stajer.Notes ?? "";
                }

                var headerRange = worksheet.Range(1, 1, 1, 10);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Columns().AdjustToContents();

                var fileName = $"Stajerler_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Dashboard verilerini hazırla
        private async Task<DashboardViewModel> GetDashboardData()
        {
            var totalStajers = await _context.Stajers.CountAsync();
            var activeStajers = await _context.Stajers
                .Where(s => s.StartDate <= DateOnly.FromDateTime(DateTime.Now) && 
                           s.EndDate >= DateOnly.FromDateTime(DateTime.Now))
                .CountAsync();

            var totalDepartments = await _context.Departmans.CountAsync();
            var totalUniversities = await _context.Universiteler.CountAsync();

            var thisMonthStajers = await _context.Stajers
                .Where(s => s.StartDate.Year == DateTime.Now.Year && 
                           s.StartDate.Month == DateTime.Now.Month)
                .CountAsync();

            var recentStajers = await _context.Stajers
                .Include(s => s.Departman)
                .Include(s => s.Universite)
                .OrderByDescending(s => s.StajerID)
                .Take(5)
                .Select(s => new StajerSummaryViewModel
                {
                    StajerID = s.StajerID,
                    FullName = s.FullName,
                    Email = s.Email,
                    DepartmanAdi = s.Departman.DepartmanAdi,
                    UniversiteAdi = s.Universite.UniversiteAdi,
                    StartDate = s.StartDate
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalStajers = totalStajers,
                ActiveStajers = activeStajers,
                TotalDepartments = totalDepartments,
                TotalUniversities = totalUniversities,
                ThisMonthStajers = thisMonthStajers,
                RecentStajers = recentStajers
            };
        }
    }
}