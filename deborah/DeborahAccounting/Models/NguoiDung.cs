using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class NguoiDung
    {
        [Key, MaxLength(50)]
        public string TenDN { get; set; } = string.Empty;

        [MaxLength(20)]
        public string MatKhau { get; set; } = string.Empty;

        [MaxLength(60)]
        public string TenNguoiDung { get; set; } = string.Empty;

        public int Quyen { get; set; }
    }
}
