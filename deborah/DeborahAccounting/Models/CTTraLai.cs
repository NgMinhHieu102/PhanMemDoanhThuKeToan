using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class CTTraLai
    {
        [MaxLength(10)]
        public string SoPhieu { get; set; } = string.Empty;

        [MaxLength(10)]
        public string MaHH { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,0)")]
        public decimal SoLuong { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal DonGia { get; set; }

        [ForeignKey("SoPhieu")]
        public TraLai? PhieuTraLai { get; set; }

        [ForeignKey("MaHH")]
        public DMHH? HangHoa { get; set; }
    }
}
