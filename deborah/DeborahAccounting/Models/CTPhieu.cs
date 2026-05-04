using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class CTPhieu
    {
        [MaxLength(6)]
        public string SoCT { get; set; } = string.Empty;

        [MaxLength(6)]
        public string? TKCo { get; set; }

        [MaxLength(6)]
        public string? TKNo { get; set; }

        [MaxLength(60)]
        public string? CTLQ { get; set; }

        [MaxLength(60)]
        public string? NoiDung { get; set; }

        [Column(TypeName = "numeric(12,0)")]
        public decimal SoTien { get; set; }

        [ForeignKey("SoCT")]
        public PhieuKT? PhieuKeToan { get; set; }
    }
}
