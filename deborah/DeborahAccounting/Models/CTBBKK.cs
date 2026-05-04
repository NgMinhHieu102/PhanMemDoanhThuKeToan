using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class CTBBKK
    {
        [MaxLength(6)]
        public string SoCT { get; set; } = string.Empty;

        [MaxLength(6)]
        public string MaVT { get; set; } = string.Empty;

        [Column(TypeName = "numeric(10,2)")]
        public decimal SoLuongKiemKe { get; set; }

        [MaxLength(60)]
        public string? LyDo { get; set; }

        [ForeignKey("SoCT")]
        public BBKK? BienBanKK { get; set; }

        [ForeignKey("MaVT")]
        public DMVT? VatTu { get; set; }
    }
}
