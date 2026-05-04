using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BaoCaoNXTController : Controller
    {
        private readonly AppDbContext _db;
        public BaoCaoNXTController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.DSVT = await _db.DMVT.OrderBy(x => x.MaVT).ToListAsync();
            ViewBag.DSKho = await _db.DMKho.OrderBy(x => x.MaKho).ToListAsync();
            return View();
        }

        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime tuNgay, DateTime denNgay, string? maVT, string maKho)
        {
            if (string.IsNullOrWhiteSpace(maKho))
                return Json(new { success = false, message = "Vui lòng chọn Mã kho" });
            if (tuNgay > denNgay)
                return Json(new { success = false, message = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" });

            var kho = await _db.DMKho.FindAsync(maKho);
            if (kho == null) return Json(new { success = false, message = "Mã kho không tồn tại" });

            string? tenVTFilter = null;
            if (!string.IsNullOrWhiteSpace(maVT))
            {
                var vt = await _db.DMVT.FindAsync(maVT);
                if (vt == null) return Json(new { success = false, message = "Mã vật tư không tồn tại" });
                tenVTFilter = vt.TenVT;
            }

            // Tồn đầu kỳ gốc
            var tonDKQuery = _db.TonDauKy.Where(t => t.MaKho == maKho);
            if (!string.IsNullOrWhiteSpace(maVT)) tonDKQuery = tonDKQuery.Where(t => t.MaVT == maVT);
            var tonDKList = await tonDKQuery.ToListAsync();

            // Phát sinh trước kỳ
            var nxTruocQ = _db.CTPHIEUNX.Include(c => c.PhieuNhapXuat)
                .Where(c => c.PhieuNhapXuat != null && c.PhieuNhapXuat.MaKho == maKho
                    && c.PhieuNhapXuat.NgayCT < tuNgay);
            if (!string.IsNullOrWhiteSpace(maVT)) nxTruocQ = nxTruocQ.Where(c => c.MaVT == maVT);
            var nxTruoc = await nxTruocQ.ToListAsync();

            // Phát sinh trong kỳ
            var nxTrongQ = _db.CTPHIEUNX.Include(c => c.PhieuNhapXuat)
                .Where(c => c.PhieuNhapXuat != null && c.PhieuNhapXuat.MaKho == maKho
                    && c.PhieuNhapXuat.NgayCT >= tuNgay && c.PhieuNhapXuat.NgayCT <= denNgay);
            if (!string.IsNullOrWhiteSpace(maVT)) nxTrongQ = nxTrongQ.Where(c => c.MaVT == maVT);
            var nxTrong = await nxTrongQ.ToListAsync();

            var allMaVT = tonDKList.Select(t => t.MaVT)
                .Union(nxTruoc.Select(c => c.MaVT))
                .Union(nxTrong.Select(c => c.MaVT))
                .Distinct().OrderBy(x => x).ToList();

            var dsVT = await _db.DMVT.Where(v => allMaVT.Contains(v.MaVT)).ToDictionaryAsync(v => v.MaVT);

            var rows = new List<object>();
            decimal tSLDK = 0, tTTDK = 0, tSLN = 0, tTTN = 0, tSLX = 0, tTTX = 0, tSLC = 0, tTTC = 0;

            foreach (var mv in allMaVT)
            {
                var tdk = tonDKList.FirstOrDefault(t => t.MaVT == mv);
                decimal slDK = tdk?.SoLuong ?? 0;
                decimal ttDK = tdk?.ThanhTien ?? 0;

                // Cộng phát sinh trước kỳ vào tồn đầu kỳ
                slDK += nxTruoc.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PN")).Sum(c => c.SoLuong);
                ttDK += nxTruoc.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PN")).Sum(c => c.ThanhTien);
                slDK -= nxTruoc.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PX")).Sum(c => c.SoLuong);
                ttDK -= nxTruoc.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PX")).Sum(c => c.ThanhTien);

                decimal slNhap = nxTrong.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PN")).Sum(c => c.SoLuong);
                decimal ttNhap = nxTrong.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PN")).Sum(c => c.ThanhTien);
                decimal slXuat = nxTrong.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PX")).Sum(c => c.SoLuong);
                decimal ttXuat = nxTrong.Where(c => c.MaVT == mv && c.SoCT.StartsWith("PX")).Sum(c => c.ThanhTien);

                decimal slCuoi = slDK + slNhap - slXuat;
                decimal ttCuoi = ttDK + ttNhap - ttXuat;

                tSLDK += slDK; tTTDK += ttDK; tSLN += slNhap; tTTN += ttNhap;
                tSLX += slXuat; tTTX += ttXuat; tSLC += slCuoi; tTTC += ttCuoi;

                dsVT.TryGetValue(mv, out var vtInfo);
                rows.Add(new {
                    maVT = mv, tenVT = vtInfo?.TenVT ?? "", dvt = vtInfo?.DVT ?? "",
                    slDK, ttDK, slNhap, ttNhap, slXuat, ttXuat, slCuoi, ttCuoi
                });
            }

            return Json(new {
                success = true, maKho, tenKho = kho.TenKho,
                maVT = maVT ?? "", tenVT = tenVTFilter ?? "",
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                tSLDK, tTTDK, tSLN, tTTN, tSLX, tTTX, tSLC, tTTC, rows
            });
        }
    }
}
