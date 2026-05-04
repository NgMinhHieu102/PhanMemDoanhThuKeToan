using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMTK
    {
        [Key, MaxLength(10)]
        public string MaTK { get; set; } = string.Empty;

        [MaxLength(128)]
        public string TenTK { get; set; } = string.Empty;

        public int CapTK { get; set; }

        [MaxLength(10)]
        public string? TKCapTren { get; set; }
    }
}
