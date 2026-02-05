using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class StajerModel
    {
        [Key]
        public int StajerID { get; set; }
        [Column(TypeName = "nvarchar(30)")]    
        public string FullName { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(50)")]

        public string Email { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(10)")]

        public string PhoneNumber { get; set; } = string.Empty;
        // Foreign Key - Üniversite ID
        public int? UniversiteID { get; set; }

        // Foreign Key - Bölüm ID
        public int? BolumID { get; set; }

        // Foreign Key - Departman ID
        public int DepartmanID { get; set; }
        
        // Navigation Properties
        [ForeignKey("UniversiteID")]
        public virtual UniversiteModel? Universite { get; set; }

        [ForeignKey("BolumID")]
        public virtual BolumModel? Bolum { get; set; }

        [ForeignKey("DepartmanID")]
        public virtual DepartmanModel? Departman { get; set; }
        public DateOnly StartDate { get; set; } 
        public DateOnly EndDate { get; set; }

        [Column(TypeName = "nvarchar(400)")]
        public string? Notes { get; set; }


    }
}
