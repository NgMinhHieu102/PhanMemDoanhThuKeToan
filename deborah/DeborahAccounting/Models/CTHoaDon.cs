using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeborahAccounting.Models
{
    public class CTHoaDon
    {
        [MaxLength(10)]
        public string SoHD { get; set; } = string.Empty;

        [MaxLength(10)]
        public string MaHH { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,0)")]
        public decimal SoLuong { get; set; }

        [Column(TypeName = "numeric(18,0)")]
        public decimal DonGia { get; set; }

        [ForeignKey("SoHD")]
        public HDHH? HoaDon { get; set; }

        [ForeignKey("MaHH")]
        public DMHH? HangHoa { get; set; }
    }
}
