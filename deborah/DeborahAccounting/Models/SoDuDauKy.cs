using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class SoDuDauKy
    {
        [Key, MaxLength(10)]
        public string MaTK { get; set; } = string.Empty;

        [Column(TypeName = "numeric(12,0)")]
        public decimal DuNo { get; set; }

        [Column(TypeName = "numeric(12,0)")]
        public decimal DuCo { get; set; }

        [ForeignKey("MaTK")]
        public DMTK? TaiKhoan { get; set; }
    }
}
