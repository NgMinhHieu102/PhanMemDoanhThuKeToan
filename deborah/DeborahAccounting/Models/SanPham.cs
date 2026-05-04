using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class SanPham
    {
        [Key, MaxLength(6)]
        public string MaSP { get; set; } = string.Empty;

        [MaxLength(60)]
        public string TenSP { get; set; } = string.Empty;
    }
}
