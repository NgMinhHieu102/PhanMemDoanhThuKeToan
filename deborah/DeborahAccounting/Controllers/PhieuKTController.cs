using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class PhieuKTController : Controller
    {
        private readonly AppDbContext _db;
        public PhieuKTController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.PhieuKT.Include(p => p.ChiTietPhieu).OrderBy(p => p.SoCT).ToListAsync();
            ViewBag.DSTK = await _db.DMTK.ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuKTDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SoCT))
                return Json(new { success = false, message = "Số chứng từ không được để trống" });
            if (await _db.PhieuKT.AnyAsync(x => x.SoCT == dto.SoCT))
                return Json(new { success = false, message = "Số chứng từ đã tồn tại" });
            _db.PhieuKT.Add(new PhieuKT { SoCT = dto.SoCT, NgayCT = dto.NgayCT, CTLQ = dto.CTLQ, LyDo = dto.LyDo });
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                    _db.CTPhieu.Add(new CTPhieu { SoCT = dto.SoCT, TKNo = ct.TKNo, TKCo = ct.TKCo, NoiDung = ct.NoiDung, SoTien = ct.SoTien });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] PhieuKTDto dto)
        {
            var p = await _db.PhieuKT.FindAsync(dto.SoCT);
            if (p == null) return Json(new { success = false, message = "Không tìm thấy" });
            p.NgayCT = dto.NgayCT; p.CTLQ = dto.CTLQ; p.LyDo = dto.LyDo;
            _db.CTPhieu.RemoveRange(await _db.CTPhieu.Where(c => c.SoCT == dto.SoCT).ToListAsync());
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                    _db.CTPhieu.Add(new CTPhieu { SoCT = dto.SoCT, TKNo = ct.TKNo, TKCo = ct.TKCo, NoiDung = ct.NoiDung, SoTien = ct.SoTien });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string id)
        {
            var p = await _db.PhieuKT.FindAsync(id);
            if (p == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CTPhieu.RemoveRange(await _db.CTPhieu.Where(c => c.SoCT == id).ToListAsync());
            _db.PhieuKT.Remove(p);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> Print(string id)
        {
            var p = await _db.PhieuKT.Include(x => x.ChiTietPhieu).FirstOrDefaultAsync(x => x.SoCT == id);
            if (p == null) return NotFound();
            return View(p);
        }

        public async Task<IActionResult> GetDetail(string id)
        {
            var p = await _db.PhieuKT.Include(x => x.ChiTietPhieu).FirstOrDefaultAsync(x => x.SoCT == id);
            if (p == null) return Json(new { });
            return Json(p, new System.Text.Json.JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles });
        }
    }

    public class PhieuKTDto
    {
        public string SoCT { get; set; } = "";
        public DateTime NgayCT { get; set; }
        public string? CTLQ { get; set; }
        public string? LyDo { get; set; }
        public List<CTPhieuDto>? ChiTiet { get; set; }
    }
    public class CTPhieuDto
    {
        public string? TKNo { get; set; }
        public string? TKCo { get; set; }
        public string? NoiDung { get; set; }
        public decimal SoTien { get; set; }
    }
}
