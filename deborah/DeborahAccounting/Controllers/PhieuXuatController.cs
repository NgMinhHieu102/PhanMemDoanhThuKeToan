using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class PhieuXuatController : Controller
    {
        private readonly AppDbContext _db;
        public PhieuXuatController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.PhieuNhapXuat
                .Where(p => p.SoCT.StartsWith("PX"))
                .Include(p => p.Kho).Include(p => p.SanPham)
                .Include(p => p.ChiTietPhieuNX)
                .OrderBy(p => p.SoCT).ToListAsync();
            ViewBag.DSKho = await _db.DMKho.ToListAsync();
            ViewBag.DSSP = await _db.SanPham.ToListAsync();
            ViewBag.DSBP = await _db.DMBP.ToListAsync();
            ViewBag.DSVT = await _db.DMVT.ToListAsync();
            ViewBag.DSTK = await _db.DMTK.ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuNhapDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SoCT))
                return Json(new { success = false, message = "Số chứng từ không được để trống" });
            if (await _db.PhieuNhapXuat.AnyAsync(x => x.SoCT == dto.SoCT))
                return Json(new { success = false, message = "Số chứng từ đã tồn tại" });
            var phieu = new PhieuNhapXuat
            {
                SoCT = dto.SoCT, NgayCT = dto.NgayCT, MaKho = dto.MaKho ?? "",
                MaNCC = "", MaSP = dto.MaSP ?? "", CTLQ = dto.CTLQ, LyDo = dto.LyDo
            };
            _db.PhieuNhapXuat.Add(phieu);
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                    _db.CTPHIEUNX.Add(new CTPHIEUNX
                    {
                        SoCT = dto.SoCT, MaVT = ct.MaVT ?? "",
                        TKNo = ct.TKNo, TKCo = ct.TKCo,
                        SoLuong = ct.SoLuong, DonGia = ct.DonGia, ThanhTien = ct.ThanhTien
                    });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] PhieuNhapDto dto)
        {
            var phieu = await _db.PhieuNhapXuat.FindAsync(dto.SoCT);
            if (phieu == null) return Json(new { success = false, message = "Không tìm thấy" });
            phieu.NgayCT = dto.NgayCT; phieu.MaKho = dto.MaKho ?? "";
            phieu.MaSP = dto.MaSP ?? ""; phieu.CTLQ = dto.CTLQ; phieu.LyDo = dto.LyDo;
            var oldCT = await _db.CTPHIEUNX.Where(c => c.SoCT == dto.SoCT).ToListAsync();
            _db.CTPHIEUNX.RemoveRange(oldCT);
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                    _db.CTPHIEUNX.Add(new CTPHIEUNX
                    {
                        SoCT = dto.SoCT, MaVT = ct.MaVT ?? "",
                        TKNo = ct.TKNo, TKCo = ct.TKCo,
                        SoLuong = ct.SoLuong, DonGia = ct.DonGia, ThanhTien = ct.ThanhTien
                    });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string id)
        {
            var phieu = await _db.PhieuNhapXuat.FindAsync(id);
            if (phieu == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CTPHIEUNX.RemoveRange(await _db.CTPHIEUNX.Where(c => c.SoCT == id).ToListAsync());
            _db.PhieuNhapXuat.Remove(phieu);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> Print(string id)
        {
            var phieu = await _db.PhieuNhapXuat
                .Include(p => p.SanPham).Include(p => p.Kho)
                .Include(p => p.ChiTietPhieuNX).ThenInclude(c => c.VatTu)
                .FirstOrDefaultAsync(p => p.SoCT == id);
            if (phieu == null) return NotFound();
            return View(phieu);
        }

        public async Task<IActionResult> GetDetail(string id)
        {
            var phieu = await _db.PhieuNhapXuat
                .Include(p => p.ChiTietPhieuNX).ThenInclude(c => c.VatTu)
                .FirstOrDefaultAsync(p => p.SoCT == id);
            if (phieu == null) return Json(new { });
            return Json(phieu, new System.Text.Json.JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles });
        }
    }
}
