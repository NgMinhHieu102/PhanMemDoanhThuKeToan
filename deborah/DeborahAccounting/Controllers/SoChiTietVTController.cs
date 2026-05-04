using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class SoChiTietVTController : Controller
    {
        private readonly AppDbContext _db;
        public SoChiTietVTController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.DSVT = await _db.DMVT.OrderBy(x => x.MaVT).ToListAsync();
            ViewBag.DSKho = await _db.DMKho.OrderBy(x => x.MaKho).ToListAsync();
            return View();
        }

        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime tuNgay, DateTime denNgay, string maVT, string maKho)
        {
            if (string.IsNullOrWhiteSpace(maVT))
                return Json(new { success = false, message = "Vui lòng chọn Mã vật tư" });
            if (string.IsNullOrWhiteSpace(maKho))
                return Json(new { success = false, message = "Vui lòng chọn Mã kho" });
            if (tuNgay > denNgay)
                return Json(new { success = false, message = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" });

            var vatTu = await _db.DMVT.FindAsync(maVT);
            var kho = await _db.DMKho.FindAsync(maKho);
            if (vatTu == null) return Json(new { success = false, message = "Mã vật tư không tồn tại" });
            if (kho == null) return Json(new { success = false, message = "Mã kho không tồn tại" });

            // Tồn đầu kỳ gốc
            var tonDK = await _db.TonDauKy
                .Where(t => t.MaVT == maVT && t.MaKho == maKho)
                .FirstOrDefaultAsync();
            decimal slTonDK = tonDK?.SoLuong ?? 0;
            decimal ttTonDK = tonDK?.ThanhTien ?? 0;

            // Phiếu trước kỳ lọc
            var truoc = await _db.CTPHIEUNX
                .Include(c => c.PhieuNhapXuat)
                .Where(c => c.MaVT == maVT
                    && c.PhieuNhapXuat != null
                    && c.PhieuNhapXuat.MaKho == maKho
                    && c.PhieuNhapXuat.NgayCT < tuNgay)
                .ToListAsync();

            decimal slNhapTruoc = truoc.Where(c => c.SoCT.StartsWith("PN")).Sum(c => c.SoLuong);
            decimal ttNhapTruoc = truoc.Where(c => c.SoCT.StartsWith("PN")).Sum(c => c.ThanhTien);
            decimal slXuatTruoc = truoc.Where(c => c.SoCT.StartsWith("PX")).Sum(c => c.SoLuong);
            decimal ttXuatTruoc = truoc.Where(c => c.SoCT.StartsWith("PX")).Sum(c => c.ThanhTien);

            decimal slTonDauKy = slTonDK + slNhapTruoc - slXuatTruoc;
            decimal ttTonDauKy = ttTonDK + ttNhapTruoc - ttXuatTruoc;

            // Chi tiết trong kỳ
            var chiTiet = await _db.CTPHIEUNX
                .Include(c => c.PhieuNhapXuat)
                .Where(c => c.MaVT == maVT
                    && c.PhieuNhapXuat != null
                    && c.PhieuNhapXuat.MaKho == maKho
                    && c.PhieuNhapXuat.NgayCT >= tuNgay
                    && c.PhieuNhapXuat.NgayCT <= denNgay)
                .OrderBy(c => c.PhieuNhapXuat!.NgayCT)
                .ThenBy(c => c.SoCT)
                .Select(c => new {
                    c.SoCT,
                    NgayCT = c.PhieuNhapXuat!.NgayCT,
                    LyDo = c.PhieuNhapXuat.LyDo ?? "",
                    LoaiPhieu = c.SoCT.StartsWith("PN") ? "N" : "X",
                    c.SoLuong,
                    c.DonGia,
                    c.ThanhTien,
                    c.TKNo,
                    c.TKCo
                })
                .ToListAsync();

            var rows = new List<object>();
            decimal slTon = slTonDauKy;
            decimal ttTon = ttTonDauKy;

            foreach (var ct in chiTiet)
            {
                decimal slNhap = ct.LoaiPhieu == "N" ? ct.SoLuong : 0;
                decimal ttNhap = ct.LoaiPhieu == "N" ? ct.ThanhTien : 0;
                decimal slXuat = ct.LoaiPhieu == "X" ? ct.SoLuong : 0;
                decimal ttXuat = ct.LoaiPhieu == "X" ? ct.ThanhTien : 0;
                slTon = slTon + slNhap - slXuat;
                ttTon = ttTon + ttNhap - ttXuat;

                rows.Add(new {
                    ct.SoCT,
                    NgayCT = ct.NgayCT.ToString("dd/MM/yyyy"),
                    ct.LyDo,
                    TKDoi = ct.LoaiPhieu == "N" ? ct.TKNo : ct.TKCo,
                    SLNhap = slNhap, DGNhap = ct.LoaiPhieu == "N" ? ct.DonGia : 0, TTNhap = ttNhap,
                    SLXuat = slXuat, DGXuat = ct.LoaiPhieu == "X" ? ct.DonGia : 0, TTXuat = ttXuat,
                    SLTon = slTon, TTTon = ttTon
                });
            }

            return Json(new {
                success = true,
                tenVT = vatTu.TenVT, dvt = vatTu.DVT, tenKho = kho.TenKho,
                maVT, maKho,
                slTonDauKy, ttTonDauKy,
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                rows
            });
        }
    }
}
