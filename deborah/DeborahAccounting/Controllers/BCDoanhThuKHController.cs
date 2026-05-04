using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BCDoanhThuKHController : Controller
    {
        private readonly AppDbContext _db;
        public BCDoanhThuKHController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();
        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay > denNgay)
                return Json(new { success = false, message = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" });

            var hoaDons = await _db.HDHH
                .Include(h => h.KhachHang)
                .Where(h => h.NgayHD >= tuNgay && h.NgayHD <= denNgay)
                .ToListAsync();

            var rows = hoaDons
                .GroupBy(h => new { h.MaKH, TenKH = h.KhachHang?.TenKH ?? "" })
                .Select(g => new {
                    maKH = g.Key.MaKH,
                    tenKH = g.Key.TenKH,
                    doanhThu = g.Sum(h => h.TienDoanhThu)
                })
                .OrderBy(r => r.maKH).ToList();

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
