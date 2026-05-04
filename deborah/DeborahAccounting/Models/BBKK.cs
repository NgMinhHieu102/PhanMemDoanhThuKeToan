using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class BBKK
    {
        [Key, MaxLength(6)]
        public string SoCT { get; set; } = string.Empty;

        [MaxLength(6)]
        public string MaKho { get; set; } = string.Empty;

        public DateTime NgayCT { get; set; }

        [MaxLength(60)]
        public string? NguoiKK1 { get; set; }

        [MaxLength(60)]
        public string? NguoiKK2 { get; set; }

        [MaxLength(60)]
        public string? NguoiKK3 { get; set; }

        [ForeignKey("MaKho")]
        public DMKho? Kho { get; set; }

        public ICollection<CTBBKK> ChiTietBBKK { get; set; } = new List<CTBBKK>();
    }
}
