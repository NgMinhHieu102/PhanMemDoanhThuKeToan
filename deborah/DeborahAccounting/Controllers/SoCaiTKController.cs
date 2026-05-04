using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class SoCaiTKController : Controller
    {
        private readonly AppDbContext _db;
        public SoCaiTKController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.DSTK = await _db.DMTK.OrderBy(x => x.MaTK).ToListAsync();
            return View();
        }

        public IActionResult Print() => View();

        [HttpPost]
        public async Task<IActionResult> Xem(DateTime tuNgay, DateTime denNgay, string maTK)
        {
            if (string.IsNullOrWhiteSpace(maTK))
                return Json(new { success = false, message = "Vui lòng chọn Tài khoản" });
            if (tuNgay > denNgay)
                return Json(new { success = false, message = "Từ ngày phải nhỏ hơn hoặc bằng Đến ngày" });

            var tk = await _db.DMTK.FindAsync(maTK);
            if (tk == null) return Json(new { success = false, message = "Tài khoản không tồn tại" });

            // Số dư đầu kỳ gốc
            var soDuDK = await _db.SoDuDauKy.FirstOrDefaultAsync(s => s.MaTK == maTK);
            decimal duNoDK = soDuDK?.DuNo ?? 0;
            decimal duCoDK = soDuDK?.DuCo ?? 0;

            // Phát sinh trước kỳ lọc từ CTPhieu
            var ctTruoc = await _db.CTPhieu
                .Include(c => c.PhieuKeToan)
                .Where(c => c.PhieuKeToan != null && c.PhieuKeToan.NgayCT < tuNgay
                    && (c.TKNo == maTK || c.TKCo == maTK))
                .ToListAsync();
            decimal psNoTruoc = ctTruoc.Where(c => c.TKNo == maTK).Sum(c => c.SoTien);
            decimal psCoTruoc = ctTruoc.Where(c => c.TKCo == maTK).Sum(c => c.SoTien);

            // Phát sinh trước kỳ từ CTPHIEUNX
            var nxTruoc = await _db.CTPHIEUNX
                .Include(c => c.PhieuNhapXuat)
                .Where(c => c.PhieuNhapXuat != null && c.PhieuNhapXuat.NgayCT < tuNgay
                    && (c.TKNo == maTK || c.TKCo == maTK))
                .ToListAsync();
            psNoTruoc += nxTruoc.Where(c => c.TKNo == maTK).Sum(c => c.ThanhTien);
            psCoTruoc += nxTruoc.Where(c => c.TKCo == maTK).Sum(c => c.ThanhTien);

            decimal duNoDauKy = duNoDK + psNoTruoc - psCoTruoc;
            decimal duCoDauKy = duCoDK + psCoTruoc - psNoTruoc;
            // Chuẩn hóa: chỉ hiển thị 1 bên
            if (duNoDauKy < 0) { duCoDauKy = -duNoDauKy; duNoDauKy = 0; }
            else if (duCoDauKy < 0) { duNoDauKy = -duCoDauKy; duCoDauKy = 0; }

            // Phát sinh trong kỳ từ CTPhieu
            var ctTrongKy = await _db.CTPhieu
                .Include(c => c.PhieuKeToan)
                .Where(c => c.PhieuKeToan != null
                    && c.PhieuKeToan.NgayCT >= tuNgay && c.PhieuKeToan.NgayCT <= denNgay
                    && (c.TKNo == maTK || c.TKCo == maTK))
                .OrderBy(c => c.PhieuKeToan!.NgayCT).ThenBy(c => c.SoCT)
                .Select(c => new ButToanDto {
                    SoCT = c.SoCT, NgayCT = c.PhieuKeToan!.NgayCT,
                    LyDo = c.NoiDung ?? c.PhieuKeToan.LyDo ?? "",
                    TKDoiUng = c.TKNo == maTK ? (c.TKCo ?? "") : (c.TKNo ?? ""),
                    No = c.TKNo == maTK ? c.SoTien : 0,
                    Co = c.TKCo == maTK ? c.SoTien : 0
                }).ToListAsync();

            // Phát sinh trong kỳ từ CTPHIEUNX
            var nxTrongKy = await _db.CTPHIEUNX
                .Include(c => c.PhieuNhapXuat)
                .Where(c => c.PhieuNhapXuat != null
                    && c.PhieuNhapXuat.NgayCT >= tuNgay && c.PhieuNhapXuat.NgayCT <= denNgay
                    && (c.TKNo == maTK || c.TKCo == maTK))
                .OrderBy(c => c.PhieuNhapXuat!.NgayCT).ThenBy(c => c.SoCT)
                .Select(c => new ButToanDto {
                    SoCT = c.SoCT, NgayCT = c.PhieuNhapXuat!.NgayCT,
                    LyDo = c.PhieuNhapXuat.LyDo ?? "",
                    TKDoiUng = c.TKNo == maTK ? (c.TKCo ?? "") : (c.TKNo ?? ""),
                    No = c.TKNo == maTK ? c.ThanhTien : 0,
                    Co = c.TKCo == maTK ? c.ThanhTien : 0
                }).ToListAsync();

            var allRows = ctTrongKy.Concat(nxTrongKy)
                .OrderBy(r => r.NgayCT).ThenBy(r => r.SoCT).ToList();

            // Phát sinh trong kỳ từ HDHH (hóa đơn GTGT bán hàng)
            var hdTrongKy = await _db.HDHH
                .Where(h => h.NgayHD >= tuNgay && h.NgayHD <= denNgay)
                .ToListAsync();
            var hdRows = new List<ButToanDto>();
            foreach (var h in hdTrongKy)
            {
                // Nợ TK thanh toán / Có TK doanh thu
                if (h.TKNoThanhToan == maTK)
                    hdRows.Add(new ButToanDto { SoCT = h.SoHD, NgayCT = h.NgayHD, LyDo = h.DienGiai ?? "Bán hàng", TKDoiUng = h.TKCoDoanhThu ?? "", No = h.TienDoanhThu, Co = 0 });
                if (h.TKCoDoanhThu == maTK)
                    hdRows.Add(new ButToanDto { SoCT = h.SoHD, NgayCT = h.NgayHD, LyDo = h.DienGiai ?? "Bán hàng", TKDoiUng = h.TKNoThanhToan ?? "", No = 0, Co = h.TienDoanhThu });
                if (h.TKCoThue == maTK && h.TienThue > 0)
                    hdRows.Add(new ButToanDto { SoCT = h.SoHD, NgayCT = h.NgayHD, LyDo = h.DienGiai ?? "Bán hàng", TKDoiUng = h.TKNoThanhToan ?? "", No = 0, Co = h.TienThue });
            }
            allRows = allRows.Concat(hdRows).ToList();

            // Phát sinh trong kỳ từ TraLai (hàng bán trả lại)
            var tlTrongKy = await _db.TraLai
                .Where(t => t.NgayPhieu >= tuNgay && t.NgayPhieu <= denNgay)
                .ToListAsync();
            var tlRows = new List<ButToanDto>();
            foreach (var t in tlTrongKy)
            {
                if (t.TKNo == maTK)
                    tlRows.Add(new ButToanDto { SoCT = t.SoPhieu, NgayCT = t.NgayPhieu, LyDo = t.DienGiai ?? "Trả lại", TKDoiUng = t.TKCo ?? "", No = t.TongTien, Co = 0 });
                if (t.TKCo == maTK)
                    tlRows.Add(new ButToanDto { SoCT = t.SoPhieu, NgayCT = t.NgayPhieu, LyDo = t.DienGiai ?? "Trả lại", TKDoiUng = t.TKNo ?? "", No = 0, Co = t.TongTien });
                if (t.TKNoThue == maTK && t.TongTien > 0)
                    tlRows.Add(new ButToanDto { SoCT = t.SoPhieu, NgayCT = t.NgayPhieu, LyDo = t.DienGiai ?? "Trả lại", TKDoiUng = t.TKCo ?? "", No = 0, Co = 0 });
            }
            allRows = allRows.Concat(tlRows).ToList();

            // Phát sinh trong kỳ từ PhieuGiamGia
            var ggTrongKy = await _db.PhieuGiamGia
                .Where(g => g.NgayPhieu >= tuNgay && g.NgayPhieu <= denNgay)
                .ToListAsync();
            var ggRows = new List<ButToanDto>();
            foreach (var g in ggTrongKy)
            {
                if (g.TKNo == maTK)
                    ggRows.Add(new ButToanDto { SoCT = g.SoPhieu, NgayCT = g.NgayPhieu, LyDo = g.DienGiai ?? "Giảm giá", TKDoiUng = g.TKCo ?? "", No = g.TongTien, Co = 0 });
                if (g.TKCo == maTK)
                    ggRows.Add(new ButToanDto { SoCT = g.SoPhieu, NgayCT = g.NgayPhieu, LyDo = g.DienGiai ?? "Giảm giá", TKDoiUng = g.TKNo ?? "", No = 0, Co = g.TongTien });
                if (g.TKNoThue == maTK && g.TongTien > 0)
                    ggRows.Add(new ButToanDto { SoCT = g.SoPhieu, NgayCT = g.NgayPhieu, LyDo = g.DienGiai ?? "Giảm giá", TKDoiUng = g.TKCo ?? "", No = 0, Co = 0 });
            }
            allRows = allRows.Concat(ggRows)
                .OrderBy(r => r.NgayCT).ThenBy(r => r.SoCT).ToList();

            var rows = allRows.Select(r => new {
                r.SoCT,
                NgayCT = r.NgayCT.ToString("dd/MM/yyyy"),
                r.LyDo, r.TKDoiUng, r.No, r.Co
            }).ToList();

            decimal tongNo = allRows.Sum(r => r.No);
            decimal tongCo = allRows.Sum(r => r.Co);
            decimal duNoCuoi = duNoDauKy + tongNo - tongCo;
            decimal duCoCuoi = duCoDauKy + tongCo - tongNo;
            if (duNoCuoi < 0) { duCoCuoi = -duNoCuoi; duNoCuoi = 0; }
            else if (duCoCuoi < 0) { duNoCuoi = -duCoCuoi; duCoCuoi = 0; }

            return Json(new {
                success = true, maTK, tenTK = tk.TenTK,
                duNoDauKy, duCoDauKy, tongNo, tongCo, duNoCuoi, duCoCuoi,
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                rows
            });
        }
    }

    public class ButToanDto
    {
        public string SoCT { get; set; } = "";
        public DateTime NgayCT { get; set; }
        public string LyDo { get; set; } = "";
        public string TKDoiUng { get; set; } = "";
        public decimal No { get; set; }
        public decimal Co { get; set; }
    }
}