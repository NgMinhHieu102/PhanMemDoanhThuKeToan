using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BBKKController : Controller
    {
        private readonly AppDbContext _db;
        public BBKKController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.BBKK.Include(b => b.Kho).OrderBy(b => b.SoCT).ToListAsync();
            ViewBag.DSKho = await _db.DMKho.ToListAsync();
            ViewBag.DSVT = await _db.DMVT.ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BBKKDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SoCT))
                return Json(new { success = false, message = "Số chứng từ không được để trống" });
            if (await _db.BBKK.AnyAsync(x => x.SoCT == dto.SoCT))
                return Json(new { success = false, message = "Số chứng từ đã tồn tại" });
            var bb = new BBKK
            {
                SoCT = dto.SoCT, MaKho = dto.MaKho ?? "", NgayCT = dto.NgayCT,
                NguoiKK1 = dto.NguoiKK1, NguoiKK2 = dto.NguoiKK2, NguoiKK3 = dto.NguoiKK3
            };
            _db.BBKK.Add(bb);
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                    _db.CTBBKK.Add(new CTBBKK { SoCT = dto.SoCT, MaVT = ct.MaVT ?? "", SoLuongKiemKe = ct.SoLuongKiemKe, LyDo = ct.LyDo });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] BBKKDto dto)
        {
            var bb = await _db.BBKK.FindAsync(dto.SoCT);
            if (bb == null) return Json(new { success = false, message = "Không tìm thấy" });
            bb.NgayCT = dto.NgayCT; bb.MaKho = dto.MaKho ?? "";
            bb.NguoiKK1 = dto.NguoiKK1; bb.NguoiKK2 = dto.NguoiKK2; bb.NguoiKK3 = dto.NguoiKK3;
            _db.CTBBKK.RemoveRange(await _db.CTBBKK.Where(c => c.SoCT == dto.SoCT).ToListAsync());
            if (dto.ChiTiet != null)
                foreach (var ct in dto.ChiTiet)
                    _db.CTBBKK.Add(new CTBBKK { SoCT = dto.SoCT, MaVT = ct.MaVT ?? "", SoLuongKiemKe = ct.SoLuongKiemKe, LyDo = ct.LyDo });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] string id)
        {
            var bb = await _db.BBKK.FindAsync(id);
            if (bb == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CTBBKK.RemoveRange(await _db.CTBBKK.Where(c => c.SoCT == id).ToListAsync());
            _db.BBKK.Remove(bb);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetDetail(string id)
        {
            var bb = await _db.BBKK.Include(b => b.ChiTietBBKK).FirstOrDefaultAsync(b => b.SoCT == id);
            if (bb == null) return Json(new { });
            return Json(bb, new System.Text.Json.JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles });
        }
    }

    public class BBKKDto
    {
        public string SoCT { get; set; } = "";
        public DateTime NgayCT { get; set; }
        public string? MaKho { get; set; }
        public string? NguoiKK1 { get; set; }
        public string? NguoiKK2 { get; set; }
        public string? NguoiKK3 { get; set; }
        public List<CTBBKKDto>? ChiTiet { get; set; }
    }
    public class CTBBKKDto
    {
        public string? MaVT { get; set; }
        public decimal SoLuongKiemKe { get; set; }
        public string? LyDo { get; set; }
    }
}
