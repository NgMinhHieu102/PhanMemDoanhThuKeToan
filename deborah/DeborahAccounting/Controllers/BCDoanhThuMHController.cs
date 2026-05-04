using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BCDoanhThuMHController : Controller
    {
        private readonly AppDbContext _db;
        public BCDoanhThuMHController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();
        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay > denNgay)
                return Json(new { success = false, message = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" });

            var chiTiet = await _db.CTHoaDon
                .Include(c => c.HoaDon)
                .Include(c => c.HangHoa)
                .Where(c => c.HoaDon != null && c.HoaDon.NgayHD >= tuNgay && c.HoaDon.NgayHD <= denNgay)
                .ToListAsync();

            var rows = chiTiet
                .GroupBy(c => new { c.MaHH, TenHH = c.HangHoa?.TenHH ?? "" })
                .Select(g => new {
                    maHH = g.Key.MaHH,
                    tenHH = g.Key.TenHH,
                    soLuong = g.Sum(c => c.SoLuong),
                    doanhThu = g.Sum(c => c.SoLuong * c.DonGia)
                })
                .OrderBy(r => r.maHH).ToList();

            return Json(new {
                success = true,
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                tongDoanhThu = rows.Sum(r => r.doanhThu),
                rows
            });
        }
    }
}
