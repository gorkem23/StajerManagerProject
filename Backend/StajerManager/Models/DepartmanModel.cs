using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class DepartmanModel
    {
        [Key]
        public int DepartmanID { get; set; }

        [Required(ErrorMessage = "Departman adı zorunludur")]
        [StringLength(50, ErrorMessage = "Departman adı en fazla 50 karakter olabilir")]
        [Column(TypeName = "nvarchar(50)")]
        public string DepartmanAdi { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        [Column(TypeName = "nvarchar(200)")]
        public string? Aciklama { get; set; }

        // Navigation Property - Bir departmanda birden fazla stajer olabilir
        public virtual ICollection<StajerModel> Stajers { get; set; } = new List<StajerModel>();
    }
}