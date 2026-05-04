using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BCTongHopDTController : Controller
    {
        private readonly AppDbContext _db;
        public BCTongHopDTController(AppDbContext db) => _db = db;

        public IActionResult Index() => View();
        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime tuNgay, DateTime denNgay)
        {
            if (tuNgay > denNgay)
                return Json(new { success = false, message = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" });

            var dtKH = await _db.HDHH
                .Include(h => h.KhachHang)
                .Where(h => h.NgayHD >= tuNgay && h.NgayHD <= denNgay)
                .GroupBy(h => new { h.MaKH, TenKH = h.KhachHang!.TenKH })
                .Select(g => new { g.Key.MaKH, g.Key.TenKH, DoanhThu = g.Sum(h => h.TienDoanhThu) })
                .ToListAsync();

            var tlKH = await _db.TraLai
                .Where(t => t.NgayPhieu >= tuNgay && t.NgayPhieu <= denNgay)
                .GroupBy(t => t.MaKH)
                .Select(g => new { MaKH = g.Key, GiamTru = g.Sum(t => t.TongTien) })
                .ToListAsync();

            var ggKH = await _db.PhieuGiamGia
                .Where(g => g.NgayPhieu >= tuNgay && g.NgayPhieu <= denNgay)
                .GroupBy(g => g.MaKH)
                .Select(g => new { MaKH = g.Key, GiamTru = g.Sum(x => x.TongTien) })
                .ToListAsync();

            var allKH = dtKH.Select(d => d.MaKH).Union(tlKH.Select(t => t.MaKH)).Union(ggKH.Select(g => g.MaKH)).Distinct().ToList();
            var dsKH = await _db.DMKH.Where(k => allKH.Contains(k.MaKH)).ToDictionaryAsync(k => k.MaKH);

            var rows = allKH.Select(maKH => {
                dsKH.TryGetValue(maKH, out var kh);
                var dt = dtKH.FirstOrDefault(d => d.MaKH == maKH)?.DoanhThu ?? 0;
                var gt = (tlKH.FirstOrDefault(t => t.MaKH == maKH)?.GiamTru ?? 0) + (ggKH.FirstOrDefault(g => g.MaKH == maKH)?.GiamTru ?? 0);
                return new { maKH, tenKH = kh?.TenKH ?? "", doanhThu = dt, giamTru = gt, doanhThuThuan = dt - gt };
            }).OrderBy(r => r.tenKH).ToList();

            return Json(new {
                success = true,
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                rows
            });
        }
    }
}
