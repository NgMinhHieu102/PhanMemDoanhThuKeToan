using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class PhieuKT
    {
        [Key, MaxLength(6)]
        public string SoCT { get; set; } = string.Empty;

        public DateTime NgayCT { get; set; }

        [MaxLength(60)]
        public string? CTLQ { get; set; }

        [MaxLength(60)]
        public string? LyDo { get; set; }

        public ICollection<CTPhieu> ChiTietPhieu { get; set; } = new List<CTPhieu>();
    }
}
