using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class HDHH
    {
        [Key, MaxLength(10)]
        public string SoHD { get; set; } = string.Empty;

        public DateTime NgayHD { get; set; }

        [MaxLength(10)]
        public string MaKH { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? TKNoThanhToan { get; set; }

        [MaxLength(10)]
        public string? TKCoDoanhThu { get; set; }

        [MaxLength(10)]
        public string? TKCoThue { get; set; }

        [MaxLength(10)]
        public string? TKChietKhau { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TienCK { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TyLeCK { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TienThanhToan { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TienDoanhThu { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TienThue { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal ThueSuat { get; set; }

        [MaxLength(50)]
        public string? DienGiai { get; set; }

        [MaxLength(50)]
        public string? HTTT { get; set; }

        [ForeignKey("MaKH")]
        public DMKH? KhachHang { get; set; }

        public ICollection<CTHoaDon> ChiTietHoaDon { get; set; } = new List<CTHoaDon>();
    }
}
