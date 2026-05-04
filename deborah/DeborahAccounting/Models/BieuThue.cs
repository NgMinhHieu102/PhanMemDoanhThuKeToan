using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class BieuThue
    {
        [Key, MaxLength(10)]
        public string MaThue { get; set; } = string.Empty;

        [MaxLength(128)]
        public string TenThue { get; set; } = string.Empty;

        [Column(TypeName = "numeric(20,0)")]
        public decimal ThueSuat { get; set; }
    }
}
