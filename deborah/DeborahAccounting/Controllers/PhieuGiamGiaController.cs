using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class PhieuGiamGiaController : Controller
    {
        private readonly AppDbContext _db;
        public PhieuGiamGiaController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.PhieuGiamGia.Include(g => g.KhachHang).Include(g => g.ChiTietGiamGia)
                .OrderBy(g => g.SoPhieu).ToListAsync();
            ViewBag.DSKH = await _db.DMKH.OrderBy(x => x.MaKH).ToListAsync();
            ViewBag.DSHH = await _db.DMHH.OrderBy(x => x.MaHH).ToListAsync();
            ViewBag.DSTK = await _db.DMTK.OrderBy(x => x.MaTK).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GiamGiaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SoPhieu))
                return Json(new { success = false, message = "Số phiếu không được để trống" });
            if (string.IsNullOrWhiteSpace(dto.MaKH))
                return Json(new { success = false, message = "Vui lòng chọn khách hàng" });
            if (dto.NgayPhieu == default)
                return Json(new { success = false, message = "Vui lòng chọn ngày lập" });
            if (dto.NgayPhieu.Date > DateTime.Today)
                return Json(new { success = false, message = "Ngày lập không được lớn hơn ngày hiện tại" });
            if (dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                return Json(new { success = false, message = "Vui lòng thêm ít nhất 1 dòng hàng hóa" });
            if (await _db.PhieuGiamGia.AnyAsync(x => x.SoPhieu == dto.SoPhieu))
                return Json(new { success = false, message = "Số phiếu đã tồn tại" });

            var gg = new PhieuGiamGia
            {
                SoPhieu = dto.SoPhieu, NgayPhieu = dto.NgayPhieu, MaKH = dto.MaKH ?? "",
                TKNo = dto.TKNo, TKNoThue = dto.TKNoThue, TKCo = dto.TKCo,
                DienGiai = dto.DienGiai, CTLQ = dto.CTLQ, TrangThai = false
            };
            decimal tongTien = 0;
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                {
                    tongTien += ct.SoLuong * ct.DonGia;
                    _db.CTGiamGia.Add(new CTGiamGia { SoPhieu = dto.SoPhieu, MaHH = ct.MaHH, SoLuong = ct.SoLuong, DonGia = ct.DonGia });
                }
            gg.TongTien = tongTien;
            _db.PhieuGiamGia.Add(gg);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var gg = await _db.PhieuGiamGia.Include(g => g.ChiTietGiamGia).FirstOrDefaultAsync(g => g.SoPhieu == id);
            if (gg == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CTGiamGia.RemoveRange(gg.ChiTietGiamGia);
            _db.PhieuGiamGia.Remove(gg);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetDetail(string id)
        {
            var gg = await _db.PhieuGiamGia.Include(g => g.ChiTietGiamGia).ThenInclude(c => c.HangHoa)
                .Include(g => g.KhachHang).FirstOrDefaultAsync(g => g.SoPhieu == id);
            if (gg == null) return Json(new { success = false });
            return Json(new {
                success = true, gg.SoPhieu, ngayPhieu = gg.NgayPhieu.ToString("yyyy-MM-dd"), gg.MaKH,
                tenKH = gg.KhachHang?.TenKH, diaChi = gg.KhachHang?.DiaChi,
                gg.TKNo, gg.TKNoThue, gg.TKCo, gg.DienGiai, gg.TongTien,
                chiTiet = gg.ChiTietGiamGia.Select(c => new { c.MaHH, tenHH = c.HangHoa?.TenHH, dvt = c.HangHoa?.Dvt, c.SoLuong, c.DonGia })
            });
        }
    }

    public class GiamGiaDto
    {
        public string SoPhieu { get; set; } = "";
        public DateTime NgayPhieu { get; set; }
        public string? MaKH { get; set; }
        public string? TKNo { get; set; }
        public string? TKNoThue { get; set; }
        public string? TKCo { get; set; }
        public string? DienGiai { get; set; }
        public string? CTLQ { get; set; }
        public List<CTGiamGiaDto>? ChiTiet { get; set; }
    }
    public class CTGiamGiaDto { public string MaHH { get; set; } = ""; public decimal SoLuong { get; set; } public decimal DonGia { get; set; } }
}
