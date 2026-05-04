using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMKho
    {
        [Key, MaxLength(6)]
        public string MaKho { get; set; } = string.Empty;

        [MaxLength(60)]
        public string TenKho { get; set; } = string.Empty;
    }
}
