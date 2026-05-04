using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class PhieuNhapXuat
    {
        [Key, MaxLength(6)]
        public string SoCT { get; set; } = string.Empty;

        public DateTime NgayCT { get; set; }

        [MaxLength(6)]
        public string MaKho { get; set; } = string.Empty;

        [MaxLength(6)]
        public string MaNCC { get; set; } = string.Empty;

        [MaxLength(6)]
        public string MaSP { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? CTLQ { get; set; }

        [MaxLength(60)]
        public string? LyDo { get; set; }

        [ForeignKey("MaKho")]
        public DMKho? Kho { get; set; }

        [ForeignKey("MaNCC")]
        public DMNCC? NhaCungCap { get; set; }

        [ForeignKey("MaSP")]
        public SanPham? SanPham { get; set; }

        public ICollection<CTPHIEUNX> ChiTietPhieuNX { get; set; } = new List<CTPHIEUNX>();
    }
}
