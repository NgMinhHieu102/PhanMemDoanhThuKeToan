using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class DMNCCController : Controller
    {
        private readonly AppDbContext _db;
        public DMNCCController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index() => View(await _db.DMNCC.OrderBy(x => x.MaNCC).ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(DMNCC model)
        {
            if (await _db.DMNCC.AnyAsync(x => x.MaNCC == model.MaNCC))
                return Json(new { success = false, message = "Mã NCC đã tồn tại" });
            _db.DMNCC.Add(model);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DMNCC model)
        {
            var item = await _db.DMNCC.FindAsync(model.MaNCC);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            item.TenNCC = model.TenNCC;
            item.DiaChi = model.DiaChi;
            item.Email = model.Email;
            item.DienThoai = model.DienThoai;
            item.MaSoThue = model.MaSoThue;
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.DMNCC.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.DMNCC.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetAll() => Json(await _db.DMNCC.OrderBy(x => x.MaNCC).ToListAsync());
    }
}
