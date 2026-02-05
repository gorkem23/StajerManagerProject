using Microsoft.AspNetCore.Mvc;
using StajerManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace StajerManager.Controllers
{
	[ApiController]
    [Authorize]
	[Route("api/[controller]")]
	public class StajerEvaluationApiController : ControllerBase
	{
		private readonly Context _context;
		private readonly ILogger<StajerEvaluationApiController> _logger;

		public StajerEvaluationApiController(Context context, ILogger<StajerEvaluationApiController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet("{stajerId:int}")]
        [Authorize]
		public async Task<IActionResult> GetStajerEvaluations(int stajerId)
		{
			var evaluations = await _context.StajerEvaluations
				.Where(e => e.StajerID == stajerId)
				.OrderBy(e => e.EvaluationDate)
				.Select(e => new
				{
					evaluationID = e.EvaluationID,
					stajerID = e.StajerID,
					evaluationDate = e.EvaluationDate.ToString("yyyy-MM-dd"),
					score = e.Score,
					notes = e.Notes,
					evaluatedBy = e.EvaluatedBy,
					createdAt = e.CreatedAt,
					updatedAt = e.UpdatedAt
				})
				.ToListAsync();

			return Ok(new { success = true, evaluations });
		}

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateStajerEvaluation([FromBody] CreateEvaluationRequest request)
        {
            try
            {
                _logger.LogInformation("CreateStajerEvaluation called with: StajerID={StajerID}, EvaluationDate={EvaluationDate}, Score={Score}",
                    request?.StajerID, request?.EvaluationDate, request?.Score);

                if (request == null)
                {
                    _logger.LogWarning("CreateStajerEvaluation: Request body is null");
                    return BadRequest(new { success = false, message = "Request body boş" });
                }

                if (request.StajerID <= 0)
                {
                    _logger.LogWarning("CreateStajerEvaluation: Invalid StajerID={StajerID}", request.StajerID);
                    return BadRequest(new { success = false, message = "Geçersiz StajerID" });
                }

                if (string.IsNullOrWhiteSpace(request.EvaluationDate))
                {
                    _logger.LogWarning("CreateStajerEvaluation: EvaluationDate is empty");
                    return BadRequest(new { success = false, message = "EvaluationDate boş olamaz" });
                }

                var stajer = await _context.Stajers.FindAsync(request.StajerID);
                if (stajer == null)
                {
                    _logger.LogWarning("CreateStajerEvaluation: Stajer not found with ID={StajerID}", request.StajerID);
                    return NotFound(new { success = false, message = "Stajer bulunamadı" });
                }

                // Tarih parse et
                if (!DateOnly.TryParse(request.EvaluationDate, out var evaluationDate))
                {
                    _logger.LogWarning("CreateStajerEvaluation: Invalid date format={EvaluationDate}", request.EvaluationDate);
                    return BadRequest(new { success = false, message = "Geçersiz tarih formatı" });
                }

                // Aynı tarih için değerlendirme var mı kontrol et (upsert pattern)
                var existingEvaluation = await _context.StajerEvaluations
                    .FirstOrDefaultAsync(e => e.StajerID == request.StajerID &&
                        e.EvaluationDate == evaluationDate);

                if (existingEvaluation != null)
                {
                    // Mevcut kayıt varsa güncelle
                    _logger.LogInformation("Updating existing evaluation ID={EvaluationID}", existingEvaluation.EvaluationID);
                    existingEvaluation.Score = request.Score;
                    existingEvaluation.Notes = request.Notes;
                    existingEvaluation.EvaluatedBy = request.EvaluatedBy;
                    existingEvaluation.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Evaluation updated successfully. ID={EvaluationID}, Score={Score}",
                        existingEvaluation.EvaluationID, existingEvaluation.Score);
                    return Ok(new { success = true, evaluationID = existingEvaluation.EvaluationID, updated = true });
                }

                // Yeni kayıt oluştur
                _logger.LogInformation("Creating new evaluation for StajerID={StajerID}, Date={EvaluationDate}, Score={Score}",
                    request.StajerID, evaluationDate, request.Score);
                var evaluation = new StajerEvaluationModel
                {
                    StajerID = request.StajerID,
                    EvaluationDate = evaluationDate,
                    Score = request.Score,
                    Notes = request.Notes,
                    EvaluatedBy = request.EvaluatedBy,
                    CreatedAt = DateTime.Now
                };

                _context.StajerEvaluations.Add(evaluation);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Evaluation created successfully. ID={EvaluationID}, Score={Score}",
                    evaluation.EvaluationID, evaluation.Score);
                return Ok(new { success = true, evaluationID = evaluation.EvaluationID, updated = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateStajerEvaluation");
                return StatusCode(500, new { success = false, message = $"Sunucu hatası: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEvaluationRequest request)
        {
            try
            {
                _logger.LogInformation("Update evaluation called: ID={EvaluationID}, Score={Score}", id, request?.Score);

                if (request == null)
                {
                    _logger.LogWarning("Update evaluation: Request body is null");
                    return BadRequest(new { success = false, message = "Request body boş" });
                }

                var evaluation = await _context.StajerEvaluations.FindAsync(id);

                if (evaluation == null)
                {
                    _logger.LogWarning("Update evaluation: Evaluation not found with ID={EvaluationID}", id);
                    return NotFound(new { success = false, message = "Değerlendirme bulunamadı" });
                }

                _logger.LogInformation("Updating evaluation ID={EvaluationID}: Old Score={OldScore}, New Score={NewScore}",
                    id, evaluation.Score, request.Score);

                evaluation.Score = request.Score;
                evaluation.Notes = request.Notes;
                evaluation.UpdatedAt = DateTime.Now;
                evaluation.EvaluatedBy = request.EvaluatedBy;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Evaluation updated successfully. ID={EvaluationID}, Score={Score}",
                    id, evaluation.Score);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update evaluation ID={EvaluationID}", id);
                return StatusCode(500, new { success = false, message = $"Sunucu hatası: {ex.Message}" });
            }
        }

		[HttpDelete("{id:int}")]
        [Authorize]
		public async Task<IActionResult> Delete(int id)
		{
			var evaluation = await _context.StajerEvaluations.FindAsync(id);

			if (evaluation == null)
			{
				return NotFound(new { success = false, message = "Değerlendirme bulunamadı" });
			}

			_context.StajerEvaluations.Remove(evaluation);
			await _context.SaveChangesAsync();
			return Ok(new { success = true });
		}
	}

	// Request Models
	public class CreateEvaluationRequest
	{
		public int StajerID { get; set; }
		public string EvaluationDate { get; set; } = string.Empty;
		public decimal? Score { get; set; }
		public string? Notes { get; set; }
		public string? EvaluatedBy { get; set; }
	}

	public class UpdateEvaluationRequest
	{
		public decimal? Score { get; set; }
		public string? Notes { get; set; }
		public string? EvaluatedBy { get; set; }
	}
}
    