using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class HoaDonTraLaiController : Controller
    {
        private readonly AppDbContext _db;
        public HoaDonTraLaiController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.TraLai.Include(t => t.KhachHang).Include(t => t.ChiTietTraLai)
                .OrderBy(t => t.SoPhieu).ToListAsync();
            ViewBag.DSKH = await _db.DMKH.OrderBy(x => x.MaKH).ToListAsync();
            ViewBag.DSHH = await _db.DMHH.OrderBy(x => x.MaHH).ToListAsync();
            ViewBag.DSTK = await _db.DMTK.OrderBy(x => x.MaTK).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TraLaiDto dto)
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
            if (await _db.TraLai.AnyAsync(x => x.SoPhieu == dto.SoPhieu))
                return Json(new { success = false, message = "Số phiếu đã tồn tại" });

            var tl = new TraLai
            {
                SoPhieu = dto.SoPhieu, NgayPhieu = dto.NgayPhieu, MaKH = dto.MaKH ?? "",
                TKNo = dto.TKNo, TKNoThue = dto.TKNoThue, TKCo = dto.TKCo,
                DienGiai = dto.DienGiai, CTLQ = dto.CTLQ, MaKho = dto.MaKho, TrangThai = false
            };

            decimal tongTien = 0;
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                {
                    tongTien += ct.SoLuong * ct.DonGia;
                    _db.CTTraLai.Add(new CTTraLai { SoPhieu = dto.SoPhieu, MaHH = ct.MaHH, SoLuong = ct.SoLuong, DonGia = ct.DonGia });
                }
            tl.TongTien = tongTien;
            _db.TraLai.Add(tl);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var tl = await _db.TraLai.Include(t => t.ChiTietTraLai).FirstOrDefaultAsync(t => t.SoPhieu == id);
            if (tl == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CTTraLai.RemoveRange(tl.ChiTietTraLai);
            _db.TraLai.Remove(tl);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetDetail(string id)
        {
            var tl = await _db.TraLai.Include(t => t.ChiTietTraLai).ThenInclude(c => c.HangHoa)
                .Include(t => t.KhachHang).FirstOrDefaultAsync(t => t.SoPhieu == id);
            if (tl == null) return Json(new { success = false });
            return Json(new {
                success = true, tl.SoPhieu, ngayPhieu = tl.NgayPhieu.ToString("yyyy-MM-dd"), tl.MaKH,
                tenKH = tl.KhachHang?.TenKH, diaChi = tl.KhachHang?.DiaChi, maSoThue = tl.KhachHang?.MaSoThue,
                tl.TKNo, tl.TKNoThue, tl.TKCo, tl.DienGiai, tl.CTLQ, tl.TongTien,
                chiTiet = tl.ChiTietTraLai.Select(c => new { c.MaHH, tenHH = c.HangHoa?.TenHH, dvt = c.HangHoa?.Dvt, c.SoLuong, c.DonGia })
            });
        }
    }

    public class TraLaiDto
    {
        public string SoPhieu { get; set; } = "";
        public DateTime NgayPhieu { get; set; }
        public string? MaKH { get; set; }
        public string? TKNo { get; set; }
        public string? TKNoThue { get; set; }
        public string? TKCo { get; set; }
        public string? DienGiai { get; set; }
        public string? CTLQ { get; set; }
        public string? MaKho { get; set; }
        public List<CTTraLaiDto>? ChiTiet { get; set; }
    }
    public class CTTraLaiDto { public string MaHH { get; set; } = ""; public decimal SoLuong { get; set; } public decimal DonGia { get; set; } }
}
