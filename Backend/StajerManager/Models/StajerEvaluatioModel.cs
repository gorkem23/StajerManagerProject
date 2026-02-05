// Backend/StajerManager/Models/StajerEvaluationModel.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class StajerEvaluationModel
    {
        [Key]
        public int EvaluationID { get; set; }
        
        [Required]
        public int StajerID { get; set; }
        
        [Required]
        [Column(TypeName = "date")]
        public DateOnly EvaluationDate { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Score { get; set; } 
        
        [Column(TypeName = "nvarchar(1000)")]
        public string? Notes { get; set; }
        
        [Column(TypeName = "nvarchar(100)")]
        public string? EvaluatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        [ForeignKey("StajerID")]
        public virtual StajerModel Stajer { get; set; } = null!;
    }
}