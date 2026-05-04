using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMTKController : Controller
    {
        private readonly AppDbContext _db;
        public DMTKController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMTK.OrderBy(x => x.MaTK).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DMTK model)
        {
            if (string.IsNullOrWhiteSpace(model.MaTK))
                return Json(new { success = false, message = "Mã tài khoản không được để trống" });
            if (string.IsNullOrWhiteSpace(model.TenTK))
                return Json(new { success = false, message = "Tên tài khoản không được để trống" });
            if (model.CapTK > 1 && string.IsNullOrWhiteSpace(model.TKCapTren))
                return Json(new { success = false, message = "TK cấp " + model.CapTK + " phải chọn TK cấp trên" });
            if (await _db.DMTK.AnyAsync(x => x.MaTK == model.MaTK))
                return Json(new { success = false, message = "Mã tài khoản đã tồn tại" });
            _db.DMTK.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] DMTK model)
        {
            var item = await _db.DMTK.FindAsync(model.MaTK);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenTK = model.TenTK;
            item.CapTK = model.CapTK;
            item.TKCapTren = model.TKCapTren;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMTK.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMTK.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
