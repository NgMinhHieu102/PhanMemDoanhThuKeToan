using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class BangGiaBan
    {
        [MaxLength(10)]
        public string MaHH { get; set; } = string.Empty;

        public DateTime NgayHLuc { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal GiaBan { get; set; }

        [ForeignKey("MaHH")]
        public DMHH? HangHoa { get; set; }
    }
}
