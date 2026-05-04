using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class TonDauKy
    {
        [MaxLength(6)]
        public string MaVT { get; set; } = string.Empty;

        [MaxLength(6)]
        public string MaKho { get; set; } = string.Empty;

        [Column(TypeName = "numeric(10,0)")]
        public decimal SoLuong { get; set; }

        [Column(TypeName = "numeric(12,0)")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaVT")]
        public DMVT? VatTu { get; set; }

        [ForeignKey("MaKho")]
        public DMKho? Kho { get; set; }
    }
}
