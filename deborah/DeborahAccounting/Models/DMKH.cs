using System.ComponentModel.DataAnnotations;

namespace DeborahAccounting.Models
{
    public class DMKH
    {
        [Key, MaxLength(10)]
        public string MaKH { get; set; } = string.Empty;

        [MaxLength(128)]
        public string TenKH { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? DienThoai { get; set; }

        [MaxLength(200)]
        public string? DiaChi { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? MaSoThue { get; set; }

        [MaxLength(50)]
        public string? SoTKNH { get; set; }
    }
}
