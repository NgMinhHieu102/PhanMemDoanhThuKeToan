using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class PhieuGiamGia
    {
        [Key, MaxLength(10)]
        public string SoPhieu { get; set; } = string.Empty;

        public DateTime NgayPhieu { get; set; }

        [MaxLength(10)]
        public string MaKH { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? TKNo { get; set; }

        [MaxLength(10)]
        public string? TKNoThue { get; set; }

        [MaxLength(10)]
        public string? TKCo { get; set; }

        [MaxLength(250)]
        public string? DienGiai { get; set; }

        [MaxLength(50)]
        public string? CTLQ { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal TongTien { get; set; }

        public bool TrangThai { get; set; }

        [ForeignKey("MaKH")]
        public DMKH? KhachHang { get; set; }

        public ICollection<CTGiamGia> ChiTietGiamGia { get; set; } = new List<CTGiamGia>();
    }
}
