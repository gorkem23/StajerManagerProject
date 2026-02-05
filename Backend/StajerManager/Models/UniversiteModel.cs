using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class UniversiteModel
    {
        [Key]
        public int UniversiteID { get; set; }

        [Required(ErrorMessage = "Üniversite adı zorunludur")]
        [StringLength(100, ErrorMessage = "Üniversite adı en fazla 100 karakter olabilir")]
        [Column(TypeName = "nvarchar(100)")]
        public string UniversiteAdi { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir")]
        [Column(TypeName = "nvarchar(200)")]
        public string? Adres { get; set; }

        [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir")]
        [Column(TypeName = "nvarchar(20)")]
        public string? Telefon { get; set; }

        [StringLength(100, ErrorMessage = "Website en fazla 100 karakter olabilir")]
        [Column(TypeName = "nvarchar(100)")]
        public string? Website { get; set; }

        [StringLength(50, ErrorMessage = "Şehir en fazla 50 karakter olabilir")]
        [Column(TypeName = "nvarchar(50)")]
        public string? Sehir { get; set; }

        [StringLength(10, ErrorMessage = "Posta kodu en fazla 10 karakter olabilir")]
        [Column(TypeName = "nvarchar(10)")]
        public string? PostaKodu { get; set; }

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;

        public bool Aktif { get; set; } = true;

        // Navigation Properties
        // Bir üniversitede birden fazla stajer olabilir
        public virtual ICollection<StajerModel> Stajers { get; set; } = new List<StajerModel>();
        
        // Bir üniversitede birden fazla bölüm olabilir
        public virtual ICollection<BolumModel> Bolumler { get; set; } = new List<BolumModel>();
    }
}
