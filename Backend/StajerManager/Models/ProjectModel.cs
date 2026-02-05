using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class ProjeModel
    {
        [Key]
        public int ProjeID { get; set; }

        [Required(ErrorMessage = "Proje adı zorunludur")]
        [StringLength(100, ErrorMessage = "Proje adı en fazla 100 karakter olabilir")]
        [Column(TypeName = "nvarchar(100)")]
        public string ProjeAdi { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        [Column(TypeName = "nvarchar(500)")]
        public string? Aciklama { get; set; }

        [StringLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string? DosyaYolu { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? DosyaAdi { get; set; }

        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;

        public DateTime? BitisTarihi { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public bool Aktif { get; set; } = true;

        // Navigation Properties
        // Many-to-many relationship with StajerModel
        public virtual ICollection<StajerProjeModel> StajerProjeler { get; set; } = new List<StajerProjeModel>();
    }
}
