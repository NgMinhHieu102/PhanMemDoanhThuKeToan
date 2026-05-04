using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class HoaDonGTGTController : Controller
    {
        private readonly AppDbContext _db;
        public HoaDonGTGTController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.HDHH.Include(h => h.KhachHang).Include(h => h.ChiTietHoaDon)
                .OrderBy(h => h.SoHD).ToListAsync();
            ViewBag.DSKH = await _db.DMKH.OrderBy(x => x.MaKH).ToListAsync();
            ViewBag.DSHH = await _db.DMHH.OrderBy(x => x.MaHH).ToListAsync();
            ViewBag.DSTK = await _db.DMTK.OrderBy(x => x.MaTK).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HoaDonDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SoHD))
                return Json(new { success = false, message = "Số hóa đơn không được để trống" });
            if (string.IsNullOrWhiteSpace(dto.MaKH))
                return Json(new { success = false, message = "Vui lòng chọn khách hàng" });
            if (dto.NgayHD == default)
                return Json(new { success = false, message = "Vui lòng chọn ngày lập" });
            if (dto.NgayHD.Date > DateTime.Today)
                return Json(new { success = false, message = "Ngày lập không được lớn hơn ngày hiện tại" });
            if (dto.ChiTiet == null || dto.ChiTiet.Count == 0)
                return Json(new { success = false, message = "Vui lòng thêm ít nhất 1 dòng hàng hóa" });
            if (await _db.HDHH.AnyAsync(x => x.SoHD == dto.SoHD))
                return Json(new { success = false, message = "Số hóa đơn đã tồn tại" });

            var hd = new HDHH
            {
                SoHD = dto.SoHD, NgayHD = dto.NgayHD, MaKH = dto.MaKH ?? "",
                TKNoThanhToan = dto.TKNoThanhToan, TKCoDoanhThu = dto.TKCoDoanhThu,
                TKCoThue = dto.TKCoThue, TKChietKhau = dto.TKChietKhau,
                ThueSuat = dto.ThueSuat, TyLeCK = dto.TyLeCK,
                DienGiai = dto.DienGiai, HTTT = dto.HTTT,
                TienCK = 0, TienThanhToan = 0, TienDoanhThu = 0, TienThue = 0
            };
            _db.HDHH.Add(hd);

            decimal tongTienHang = 0;
            if (dto.ChiTiet != null)
            {
                foreach (var ct in dto.ChiTiet)
                {
                    decimal tt = ct.SoLuong * ct.DonGia;
                    tongTienHang += tt;
                    _db.CTHoaDon.Add(new CTHoaDon { SoHD = dto.SoHD, MaHH = ct.MaHH, SoLuong = ct.SoLuong, DonGia = ct.DonGia });
                }
            }
            hd.TienDoanhThu = tongTienHang;
            hd.TienThue = Math.Round(tongTienHang * dto.ThueSuat / 100);
            hd.TienCK = Math.Round(tongTienHang * dto.TyLeCK / 100);
            hd.TienThanhToan = tongTienHang + hd.TienThue - hd.TienCK;

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] HoaDonDto dto)
        {
            var hd = await _db.HDHH.Include(h => h.ChiTietHoaDon).FirstOrDefaultAsync(h => h.SoHD == dto.SoHD);
            if (hd == null) return Json(new { success = false, message = "Không tìm thấy" });

            hd.NgayHD = dto.NgayHD; hd.MaKH = dto.MaKH ?? "";
            hd.TKNoThanhToan = dto.TKNoThanhToan; hd.TKCoDoanhThu = dto.TKCoDoanhThu;
            hd.TKCoThue = dto.TKCoThue; hd.TKChietKhau = dto.TKChietKhau;
            hd.ThueSuat = dto.ThueSuat; hd.TyLeCK = dto.TyLeCK;
            hd.DienGiai = dto.DienGiai; hd.HTTT = dto.HTTT;

            _db.CTHoaDon.RemoveRange(hd.ChiTietHoaDon);
            decimal tongTienHang = 0;
            if (dto.ChiTiet != null)
            {
                foreach (var ct in dto.ChiTiet)
                {
                    decimal tt = ct.SoLuong * ct.DonGia;
                    tongTienHang += tt;
                    _db.CTHoaDon.Add(new CTHoaDon { SoHD = dto.SoHD, MaHH = ct.MaHH, SoLuong = ct.SoLuong, DonGia = ct.DonGia });
                }
            }
            hd.TienDoanhThu = tongTienHang;
            hd.TienThue = Math.Round(tongTienHang * dto.ThueSuat / 100);
            hd.TienCK = Math.Round(tongTienHang * dto.TyLeCK / 100);
            hd.TienThanhToan = tongTienHang + hd.TienThue - hd.TienCK;

            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var hd = await _db.HDHH.Include(h => h.ChiTietHoaDon).FirstOrDefaultAsync(h => h.SoHD == id);
            if (hd == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CTHoaDon.RemoveRange(hd.ChiTietHoaDon);
            _db.HDHH.Remove(hd);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetDetail(string id)
        {
            var hd = await _db.HDHH.Include(h => h.ChiTietHoaDon).ThenInclude(c => c.HangHoa)
                .Include(h => h.KhachHang).FirstOrDefaultAsync(h => h.SoHD == id);
            if (hd == null) return Json(new { success = false });
            return Json(new {
                success = true, hd.SoHD, ngayHD = hd.NgayHD.ToString("yyyy-MM-dd"), hd.MaKH,
                tenKH = hd.KhachHang?.TenKH, diaChi = hd.KhachHang?.DiaChi, maSoThue = hd.KhachHang?.MaSoThue,
                hd.TKNoThanhToan, hd.TKCoDoanhThu, hd.TKCoThue, hd.TKChietKhau,
                hd.ThueSuat, hd.TyLeCK, hd.DienGiai, hd.HTTT,
                hd.TienDoanhThu, hd.TienThue, hd.TienCK, hd.TienThanhToan,
                chiTiet = hd.ChiTietHoaDon.Select(c => new { c.MaHH, tenHH = c.HangHoa?.TenHH, dvt = c.HangHoa?.Dvt, c.SoLuong, c.DonGia, thanhTien = c.SoLuong * c.DonGia })
            });
        }

        public async Task<IActionResult> Print(string id)
        {
            var hd = await _db.HDHH.Include(h => h.ChiTietHoaDon).ThenInclude(c => c.HangHoa)
                .Include(h => h.KhachHang).FirstOrDefaultAsync(h => h.SoHD == id);
            if (hd == null) return NotFound();
            return View(hd);
        }
    }

    public class HoaDonDto
    {
        public string SoHD { get; set; } = "";
        public DateTime NgayHD { get; set; }
        public string? MaKH { get; set; }
        public string? TKNoThanhToan { get; set; }
        public string? TKCoDoanhThu { get; set; }
        public string? TKCoThue { get; set; }
        public string? TKChietKhau { get; set; }
        public decimal ThueSuat { get; set; }
        public decimal TyLeCK { get; set; }
        public string? DienGiai { get; set; }
        public string? HTTT { get; set; }
        public List<CTHoaDonDto>? ChiTiet { get; set; }
    }

    public class CTHoaDonDto
    {
        public string MaHH { get; set; } = "";
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
    }
}
