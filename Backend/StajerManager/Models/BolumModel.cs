using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class BolumModel
    {
        [Key]
        public int BolumID { get; set; }

        [Required(ErrorMessage = "Bölüm adı zorunludur")]
        [StringLength(100, ErrorMessage = "Bölüm adı en fazla 100 karakter olabilir")]
        [Column(TypeName = "nvarchar(100)")]
        public string BolumAdi { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "Bölüm kodu en fazla 10 karakter olabilir")]
        [Column(TypeName = "nvarchar(10)")]
        public string? BolumKodu { get; set; }

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir")]
        [Column(TypeName = "nvarchar(200)")]
        public string? Aciklama { get; set; }

        [StringLength(50, ErrorMessage = "Fakülte adı en fazla 50 karakter olabilir")]
        [Column(TypeName = "nvarchar(50)")]
        public string? Fakulte { get; set; }

        [StringLength(20, ErrorMessage = "Eğitim süresi en fazla 20 karakter olabilir")]
        [Column(TypeName = "nvarchar(20)")]
        public string? EgitimSuresi { get; set; }

        [StringLength(20, ErrorMessage = "Eğitim türü en fazla 20 karakter olabilir")]
        [Column(TypeName = "nvarchar(20)")]
        public string? EgitimTuru { get; set; } // Önlisans, Lisans, Yüksek Lisans, Doktora

        // Foreign Key - Üniversite ID (Her bölüm bir üniversiteye ait olmalı)
        [Required]
        public int UniversiteID { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public bool Aktif { get; set; } = true;

        // Navigation Properties
        [ForeignKey("UniversiteID")]
        public virtual UniversiteModel Universite { get; set; } = null!;

        // Navigation Property - Bir bölümde birden fazla stajer olabilir
        public virtual ICollection<StajerModel> Stajers { get; set; } = new List<StajerModel>();
    }
}
