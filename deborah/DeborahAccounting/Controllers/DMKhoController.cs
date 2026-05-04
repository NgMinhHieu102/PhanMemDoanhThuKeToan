using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMKhoController : Controller
    {
        private readonly AppDbContext _db;
        public DMKhoController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMKho.OrderBy(x => x.MaKho).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DMKho model)
        {
            if (string.IsNullOrWhiteSpace(model.MaKho))
                return Json(new { success = false, message = "Mã kho không được để trống" });
            if (string.IsNullOrWhiteSpace(model.TenKho))
                return Json(new { success = false, message = "Tên kho không được để trống" });
            if (await _db.DMKho.AnyAsync(x => x.MaKho == model.MaKho))
                return Json(new { success = false, message = "Mã kho đã tồn tại" });
            _db.DMKho.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] DMKho model)
        {
            var item = await _db.DMKho.FindAsync(model.MaKho);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenKho = model.TenKho;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMKho.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMKho.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.DMKho.OrderBy(x => x.MaKho).ToListAsync());
    }
}
