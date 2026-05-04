using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class SoChiTietTKController : Controller
    {
        private readonly AppDbContext _db;
        public SoChiTietTKController(AppDbContext db) => _db = db;

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

            // Phát sinh trước kỳ từ CTPhieu
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

            // Số dư đầu kỳ thực tế = gốc + phát sinh trước kỳ (tính theo bên Nợ)
            decimal duDauKy = (duNoDK - duCoDK) + (psNoTruoc - psCoTruoc);

            // Phát sinh trong kỳ từ CTPhieu
            var ctTrongKy = await _db.CTPhieu
                .Include(c => c.PhieuKeToan)
                .Where(c => c.PhieuKeToan != null
                    && c.PhieuKeToan.NgayCT >= tuNgay && c.PhieuKeToan.NgayCT <= denNgay
                    && (c.TKNo == maTK || c.TKCo == maTK))
                .OrderBy(c => c.PhieuKeToan!.NgayCT).ThenBy(c => c.SoCT)
                .Select(c => new ButToanRow {
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
                .Select(c => new ButToanRow {
                    SoCT = c.SoCT, NgayCT = c.PhieuNhapXuat!.NgayCT,
                    LyDo = c.PhieuNhapXuat.LyDo ?? "",
                    TKDoiUng = c.TKNo == maTK ? (c.TKCo ?? "") : (c.TKNo ?? ""),
                    No = c.TKNo == maTK ? c.ThanhTien : 0,
                    Co = c.TKCo == maTK ? c.ThanhTien : 0
                }).ToListAsync();

            var allRows = ctTrongKy.Concat(nxTrongKy)
                .OrderBy(r => r.NgayCT).ThenBy(r => r.SoCT).ToList();

            // Phát sinh từ HDHH
            var hdRows = new List<ButToanRow>();
            var hdTrongKy = await _db.HDHH.Where(h => h.NgayHD >= tuNgay && h.NgayHD <= denNgay).ToListAsync();
            foreach (var h in hdTrongKy)
            {
                if (h.TKNoThanhToan == maTK)
                    hdRows.Add(new ButToanRow { SoCT = h.SoHD, NgayCT = h.NgayHD, LyDo = h.DienGiai ?? "Bán hàng", TKDoiUng = h.TKCoDoanhThu ?? "", No = h.TienDoanhThu, Co = 0 });
                if (h.TKCoDoanhThu == maTK)
                    hdRows.Add(new ButToanRow { SoCT = h.SoHD, NgayCT = h.NgayHD, LyDo = h.DienGiai ?? "Bán hàng", TKDoiUng = h.TKNoThanhToan ?? "", No = 0, Co = h.TienDoanhThu });
                if (h.TKCoThue == maTK && h.TienThue > 0)
                    hdRows.Add(new ButToanRow { SoCT = h.SoHD, NgayCT = h.NgayHD, LyDo = h.DienGiai ?? "Bán hàng", TKDoiUng = h.TKNoThanhToan ?? "", No = 0, Co = h.TienThue });
            }
            // Phát sinh từ TraLai
            var tlRows = new List<ButToanRow>();
            var tlTrongKy = await _db.TraLai.Where(t => t.NgayPhieu >= tuNgay && t.NgayPhieu <= denNgay).ToListAsync();
            foreach (var t in tlTrongKy)
            {
                if (t.TKNo == maTK) tlRows.Add(new ButToanRow { SoCT = t.SoPhieu, NgayCT = t.NgayPhieu, LyDo = t.DienGiai ?? "Trả lại", TKDoiUng = t.TKCo ?? "", No = t.TongTien, Co = 0 });
                if (t.TKCo == maTK) tlRows.Add(new ButToanRow { SoCT = t.SoPhieu, NgayCT = t.NgayPhieu, LyDo = t.DienGiai ?? "Trả lại", TKDoiUng = t.TKNo ?? "", No = 0, Co = t.TongTien });
            }
            // Phát sinh từ PhieuGiamGia
            var ggRows = new List<ButToanRow>();
            var ggTrongKy = await _db.PhieuGiamGia.Where(g => g.NgayPhieu >= tuNgay && g.NgayPhieu <= denNgay).ToListAsync();
            foreach (var g in ggTrongKy)
            {
                if (g.TKNo == maTK) ggRows.Add(new ButToanRow { SoCT = g.SoPhieu, NgayCT = g.NgayPhieu, LyDo = g.DienGiai ?? "Giảm giá", TKDoiUng = g.TKCo ?? "", No = g.TongTien, Co = 0 });
                if (g.TKCo == maTK) ggRows.Add(new ButToanRow { SoCT = g.SoPhieu, NgayCT = g.NgayPhieu, LyDo = g.DienGiai ?? "Giảm giá", TKDoiUng = g.TKNo ?? "", No = 0, Co = g.TongTien });
            }
            allRows = allRows.Concat(hdRows).Concat(tlRows).Concat(ggRows)
                .OrderBy(r => r.NgayCT).ThenBy(r => r.SoCT).ToList();

            // Build rows với cột Tồn lũy kế
            decimal ton = duDauKy;
            var rows = allRows.Select(r => {
                ton = ton + r.No - r.Co;
                return new {
                    r.SoCT,
                    NgayCT = r.NgayCT.ToString("dd/MM/yyyy"),
                    r.LyDo, r.TKDoiUng, r.No, r.Co, Ton = ton
                };
            }).ToList();

            decimal tongNo = allRows.Sum(r => r.No);
            decimal tongCo = allRows.Sum(r => r.Co);
            decimal duCuoiKy = duDauKy + tongNo - tongCo;

            // Hiển thị số dư đầu kỳ: nếu dương -> bên Nợ, âm -> bên Có
            decimal duNoDauKy = duDauKy >= 0 ? duDauKy : 0;
            decimal duCoDauKy = duDauKy < 0 ? -duDauKy : 0;
            decimal duNoCuoi = duCuoiKy >= 0 ? duCuoiKy : 0;
            decimal duCoCuoi = duCuoiKy < 0 ? -duCuoiKy : 0;

            return Json(new {
                success = true, maTK, tenTK = tk.TenTK,
                duNoDauKy, duCoDauKy, duDauKy,
                tongNo, tongCo, duNoCuoi, duCoCuoi, duCuoiKy,
                tuNgay = tuNgay.ToString("dd/MM/yyyy"),
                denNgay = denNgay.ToString("dd/MM/yyyy"),
                rows
            });
        }
    }

    public class ButToanRow
    {
        public string SoCT { get; set; } = "";
        public DateTime NgayCT { get; set; }
        public string LyDo { get; set; } = "";
        public string TKDoiUng { get; set; } = "";
        public decimal No { get; set; }
        public decimal Co { get; set; }
    }
}
