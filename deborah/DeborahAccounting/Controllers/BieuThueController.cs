using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BieuThueController : Controller
    {
        private readonly AppDbContext _db;
        public BieuThueController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.BieuThue.OrderBy(x => x.MaThue).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BieuThue model)
        {
            if (string.IsNullOrWhiteSpace(model.MaThue))
                return Json(new { success = false, message = "Mã thuế không được để trống" });
            if (string.IsNullOrWhiteSpace(model.TenThue))
                return Json(new { success = false, message = "Tên thuế không được để trống" });
            if (await _db.BieuThue.AnyAsync(x => x.MaThue == model.MaThue))
                return Json(new { success = false, message = "Mã thuế đã tồn tại" });
            _db.BieuThue.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] BieuThue model)
        {
            var item = await _db.BieuThue.FindAsync(model.MaThue);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenThue = model.TenThue;
            item.ThueSuat = model.ThueSuat;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.BieuThue.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.BieuThue.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
