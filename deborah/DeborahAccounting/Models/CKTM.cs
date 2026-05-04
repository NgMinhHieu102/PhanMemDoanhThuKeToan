using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class CKTM
    {
        [Key, MaxLength(10)]
        public string SoBang { get; set; } = string.Empty;

        [MaxLength(10)]
        public string MaKH { get; set; } = string.Empty;

        public DateTime NgayHLuc { get; set; }

        [Column(TypeName = "numeric(10,0)")]
        public decimal TyleCK { get; set; }

        [Column(TypeName = "numeric(10,0)")]
        public decimal TienCK { get; set; }

        [ForeignKey("MaKH")]
        public DMKH? KhachHang { get; set; }
    }
}
