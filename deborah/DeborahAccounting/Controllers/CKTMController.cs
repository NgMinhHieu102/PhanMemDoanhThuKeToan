using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class CKTMController : Controller
    {
        private readonly AppDbContext _db;
        public CKTMController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.CKTM.Include(c => c.KhachHang).OrderBy(c => c.SoBang).ToListAsync();
            ViewBag.DSKH = await _db.DMKH.OrderBy(x => x.MaKH).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CKTM model)
        {
            if (await _db.CKTM.AnyAsync(x => x.SoBang == model.SoBang))
                return Json(new { success = false, message = "Số bảng đã tồn tại" });
            _db.CKTM.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CKTM model)
        {
            var item = await _db.CKTM.FindAsync(model.SoBang);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.MaKH = model.MaKH; item.NgayHLuc = model.NgayHLuc;
            item.TyleCK = model.TyleCK; item.TienCK = model.TienCK;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.CKTM.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.CKTM.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
