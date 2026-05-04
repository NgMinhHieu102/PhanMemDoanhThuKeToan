using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMBP
    {
        [Key, MaxLength(6)]
        public string MaBP { get; set; } = string.Empty;

        [MaxLength(60)]
        public string TenBP { get; set; } = string.Empty;
    }
}
