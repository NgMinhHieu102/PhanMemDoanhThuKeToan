using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class SanPhamController : Controller
    {
        private readonly AppDbContext _db;
        public SanPhamController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.SanPham.OrderBy(x => x.MaSP).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(SanPham model)
        {
            if (await _db.SanPham.AnyAsync(x => x.MaSP == model.MaSP))
                return Json(new { success = false, message = "Mã sản phẩm đã tồn tại" });
            _db.SanPham.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SanPham model)
        {
            var item = await _db.SanPham.FindAsync(model.MaSP);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenSP = model.TenSP;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.SanPham.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.SanPham.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.SanPham.OrderBy(x => x.MaSP).ToListAsync());
    }
}
