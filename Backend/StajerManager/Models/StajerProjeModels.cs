using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajerManager.Models
{
    public class StajerProjeModel
    {
        [Key]
        public int StajerProjeID { get; set; }

        [Required]
        public int StajerID { get; set; }

        [Required]
        public int ProjeID { get; set; }

        [ForeignKey("StajerID")]
        public virtual StajerModel Stajer { get; set; } = null!;

        [ForeignKey("ProjeID")]
        public virtual ProjeModel Proje { get; set; } = null!;
    }
}

