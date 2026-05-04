using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class CTPHIEUNX
    {
        [MaxLength(6)]
        public string SoCT { get; set; } = string.Empty;

        [MaxLength(6)]
        public string MaVT { get; set; } = string.Empty;

        [MaxLength(6)]
        public string? TKNo { get; set; }

        [MaxLength(6)]
        public string? TKCo { get; set; }

        [Column(TypeName = "numeric(10,0)")]
        public decimal SoLuong { get; set; }

        [Column(TypeName = "numeric(10,0)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "numeric(12,0)")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("SoCT")]
        public PhieuNhapXuat? PhieuNhapXuat { get; set; }

        [ForeignKey("MaVT")]
        public DMVT? VatTu { get; set; }
    }
}
