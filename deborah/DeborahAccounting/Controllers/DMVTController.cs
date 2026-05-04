using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMVTController : Controller
    {
        private readonly AppDbContext _db;
        public DMVTController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMVT.OrderBy(x => x.MaVT).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(DMVT model)
        {
            if (await _db.DMVT.AnyAsync(x => x.MaVT == model.MaVT))
                return Json(new { success = false, message = "Mã vật tư đã tồn tại" });
            _db.DMVT.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DMVT model)
        {
            var item = await _db.DMVT.FindAsync(model.MaVT);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenVT = model.TenVT;
            item.DVT = model.DVT;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMVT.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMVT.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.DMVT.OrderBy(x => x.MaVT).ToListAsync());
    }
}
