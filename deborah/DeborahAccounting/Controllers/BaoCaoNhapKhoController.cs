using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BaoCaoNhapKhoController : Controller
    {
        private readonly AppDbContext _db;
        public BaoCaoNhapKhoController(AppDbContext db) => _db = db;

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

            // Tổng hợp nhập trong kỳ (phiếu nhập PN)
            var nhap = await _db.CTPHIEUNX
                .Include(c => c.PhieuNhapXuat)
                .Include(c => c.VatTu)
                .Where(c => c.MaVT == maVT
                    && c.PhieuNhapXuat != null
                    && c.PhieuNhapXuat.MaKho == maKho
                    && c.SoCT.StartsWith("PN")
                    && c.PhieuNhapXuat.NgayCT >= tuNgay
                    && c.PhieuNhapXuat.NgayCT <= denNgay)
                .ToListAsync();

            decimal tongSL = nhap.Sum(c => c.SoLuong);
            decimal tongTT = nhap.Sum(c => c.ThanhTien);

            var rows = new List<object>();
            if (tongSL > 0)
            {
                rows.Add(new {
                    maVT, tenVT = vatTu.TenVT, dvt = vatTu.DVT,
                    soLuong = tongSL, thanhTien = tongTT
                });
            }

            return Json(new {
                success = true, maVT, tenVT = vatTu.TenVT, dvt = vatTu.DVT,
                maKho, tenKho = kho.TenKho,
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                tongSL, tongTT, rows
            });
        }
    }
}
