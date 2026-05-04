using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMBPController : Controller
    {
        private readonly AppDbContext _db;
        public DMBPController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMBP.OrderBy(x => x.MaBP).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(DMBP model)
        {
            if (await _db.DMBP.AnyAsync(x => x.MaBP == model.MaBP))
                return Json(new { success = false, message = "Mã bộ phận đã tồn tại" });
            _db.DMBP.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DMBP model)
        {
            var item = await _db.DMBP.FindAsync(model.MaBP);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenBP = model.TenBP;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMBP.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMBP.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.DMBP.OrderBy(x => x.MaBP).ToListAsync());
    }
}
