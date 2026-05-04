using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMKHController : Controller
    {
        private readonly AppDbContext _db;
        public DMKHController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMKH.OrderBy(x => x.MaKH).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DMKH model)
        {
            if (string.IsNullOrWhiteSpace(model.MaKH))
                return Json(new { success = false, message = "Mã khách hàng không được để trống" });
            if (string.IsNullOrWhiteSpace(model.TenKH))
                return Json(new { success = false, message = "Tên khách hàng không được để trống" });
            if (await _db.DMKH.AnyAsync(x => x.MaKH == model.MaKH))
                return Json(new { success = false, message = "Mã khách hàng đã tồn tại" });
            _db.DMKH.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] DMKH model)
        {
            var item = await _db.DMKH.FindAsync(model.MaKH);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenKH = model.TenKH;
            item.DienThoai = model.DienThoai;
            item.DiaChi = model.DiaChi;
            item.Email = model.Email;
            item.MaSoThue = model.MaSoThue;
            item.SoTKNH = model.SoTKNH;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMKH.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMKH.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.DMKH.OrderBy(x => x.MaKH).ToListAsync());
    }
}
