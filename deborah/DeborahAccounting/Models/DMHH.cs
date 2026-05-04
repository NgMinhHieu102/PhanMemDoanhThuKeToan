using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMHH
    {
        [Key, MaxLength(10)]
        public string MaHH { get; set; } = string.Empty;

        [MaxLength(128)]
        public string TenHH { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Dvt { get; set; }
    }
}
