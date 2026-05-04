using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMVT
    {
        [Key, MaxLength(6)]
        public string MaVT { get; set; } = string.Empty;

        [MaxLength(60)]
        public string TenVT { get; set; } = string.Empty;

        [MaxLength(10)]
        public string DVT { get; set; } = string.Empty;
    }
}
