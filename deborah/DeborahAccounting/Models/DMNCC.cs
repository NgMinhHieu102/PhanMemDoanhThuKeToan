using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMNCC
    {
        [Key, MaxLength(6)]
        public string MaNCC { get; set; } = string.Empty;

        [MaxLength(60)]
        public string TenNCC { get; set; } = string.Empty;

        [MaxLength(60)]
        public string DiaChi { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(12)]
        public string DienThoai { get; set; } = string.Empty;

        [MaxLength(14)]
        public string MaSoThue { get; set; } = string.Empty;
    }
}
