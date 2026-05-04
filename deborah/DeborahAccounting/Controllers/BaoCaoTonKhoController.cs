using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BaoCaoTonKhoController : Controller
    {
        private readonly AppDbContext _db;
        public BaoCaoTonKhoController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.DSVT = await _db.DMVT.OrderBy(x => x.MaVT).ToListAsync();
            ViewBag.DSKho = await _db.DMKho.OrderBy(x => x.MaKho).ToListAsync();
            return View();
        }

        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime denNgay, string? maVT, string maKho)
        {
            if (string.IsNullOrWhiteSpace(maKho))
                return Json(new { success = false, message = "Vui lòng chọn Mã kho" });

            var kho = await _db.DMKho.FindAsync(maKho);
            if (kho == null) return Json(new { success = false, message = "Mã kho không tồn tại" });

            string? tenVTFilter = null;
            if (!string.IsNullOrWhiteSpace(maVT))
            {
                var vt = await _db.DMVT.FindAsync(maVT);
                if (vt == null) return Json(new { success = false, message = "Mã vật tư không tồn tại" });
                tenVTFilter = vt.TenVT;
            }

            // Lấy tồn đầu kỳ gốc
            var tonDKQuery = _db.TonDauKy.Where(t => t.MaKho == maKho);
            if (!string.IsNullOrWhiteSpace(maVT)) tonDKQuery = tonDKQuery.Where(t => t.MaVT == maVT);
            var tonDKList = await tonDKQuery.ToListAsync();

            // Lấy phát sinh nhập/xuất đến ngày
            var nxQuery = _db.CTPHIEUNX
                .Include(c => c.PhieuNhapXuat)
                .Where(c => c.PhieuNhapXuat != null
                    && c.PhieuNhapXuat.MaKho == maKho
                    && c.PhieuNhapXuat.NgayCT <= denNgay);
            if (!string.IsNullOrWhiteSpace(maVT)) nxQuery = nxQuery.Where(c => c.MaVT == maVT);
            var nxList = await nxQuery.ToListAsync();

            // Tổng hợp theo mã vật tư
            var allMaVT = tonDKList.Select(t => t.MaVT)
                .Union(nxList.Select(c => c.MaVT)).Distinct().OrderBy(x => x).ToList();

            var dsVT = await _db.DMVT.Where(v => allMaVT.Contains(v.MaVT)).ToDictionaryAsync(v => v.MaVT);

            var rows = new List<object>();
            decimal tongSL = 0, tongTT = 0;

            foreach (var mv in allMaVT)
            {
                var tdk = tonDKList.FirstOrDefault(t => t.MaVT == mv);
                decimal slDK = tdk?.SoLuong ?? 0;
                decimal ttDK = tdk?.ThanhTien ?? 0;

                decimal slNhap = nxList.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PN")).Sum(c => c.SoLuong);
                decimal ttNhap = nxList.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PN")).Sum(c => c.ThanhTien);
                decimal slXuat = nxList.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PX")).Sum(c => c.SoLuong);
                decimal ttXuat = nxList.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PX")).Sum(c => c.ThanhTien);

                decimal slTon = slDK + slNhap - slXuat;
                decimal ttTon = ttDK + ttNhap - ttXuat;
                tongSL += slTon;
                tongTT += ttTon;

                dsVT.TryGetValue(mv, out var vtInfo);
                rows.Add(new {
                    maVT = mv,
                    tenVT = vtInfo?.TenVT ?? "",
                    dvt = vtInfo?.DVT ?? "",
                    soLuong = slTon,
                    thanhTien = ttTon
                });
            }

            return Json(new {
                success = true,
                maKho, tenKho = kho.TenKho,
                maVT = maVT ?? "", tenVT = tenVTFilter ?? "",
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                tongSL, tongTT, rows
            });
        }
    }
}
