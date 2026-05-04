using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;
using DeborahAccounting.Filters;

namespace DeborahAccounting.Controllers
{
    [Authorize, AdminOnly]
    public class NguoiDungController : Controller
    {
        private readonly AppDbContext _db;
        public NguoiDungController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.NguoiDung.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NguoiDung model)
        {
            if (string.IsNullOrWhiteSpace(model.TenDN))
                return Json(new { success = false, message = "Tên đăng nhập không được để trống" });
            if (string.IsNullOrWhiteSpace(model.MatKhau))
                return Json(new { success = false, message = "Mật khẩu không được để trống" });
            if (await _db.NguoiDung.AnyAsync(x => x.TenDN == model.TenDN))
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại" });
            _db.NguoiDung.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] NguoiDung model)
        {
            var item = await _db.NguoiDung.FindAsync(model.TenDN);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenNguoiDung = model.TenNguoiDung;
            item.MatKhau = model.MatKhau;
            item.Quyen = model.Quyen;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.NguoiDung.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.NguoiDung.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.NguoiDung.ToListAsync());
    }
}
