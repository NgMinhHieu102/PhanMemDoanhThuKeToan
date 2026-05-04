using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMHHController : Controller
    {
        private readonly AppDbContext _db;
        public DMHHController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMHH.OrderBy(x => x.MaHH).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DMHH model)
        {
            if (string.IsNullOrWhiteSpace(model.MaHH))
                return Json(new { success = false, message = "Mã hàng hóa không được để trống" });
            if (string.IsNullOrWhiteSpace(model.TenHH))
                return Json(new { success = false, message = "Tên hàng hóa không được để trống" });
            if (await _db.DMHH.AnyAsync(x => x.MaHH == model.MaHH))
                return Json(new { success = false, message = "Mã hàng hóa đã tồn tại" });
            _db.DMHH.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] DMHH model)
        {
            var item = await _db.DMHH.FindAsync(model.MaHH);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenHH = model.TenHH;
            item.Dvt = model.Dvt;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMHH.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMHH.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.DMHH.OrderBy(x => x.MaHH).ToListAsync());
    }
}
