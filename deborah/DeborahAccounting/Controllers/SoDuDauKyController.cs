using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class SoDuDauKyController : Controller
    {
        private readonly AppDbContext _db;
        public SoDuDauKyController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.SoDuDauKy.Include(s => s.TaiKhoan).OrderBy(s => s.MaTK).ToListAsync();
            ViewBag.DSTK = await _db.DMTK.OrderBy(x => x.MaTK).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] SoDuDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaTK))
                return Json(new { success = false, message = "Vui lòng chọn tài khoản" });
            if (dto.DuNo > 0 && dto.DuCo > 0)
                return Json(new { success = false, message = "Chỉ được nhập Dư Nợ hoặc Dư Có, không được nhập cả hai" });

            var item = await _db.SoDuDauKy.FindAsync(dto.MaTK);
            if (item != null)
            {
                item.DuNo = dto.DuNo;
                item.DuCo = dto.DuCo;
            }
            else
            {
                _db.SoDuDauKy.Add(new SoDuDauKy { MaTK = dto.MaTK, DuNo = dto.DuNo, DuCo = dto.DuCo });
            }
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var item = await _db.SoDuDauKy.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.SoDuDauKy.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

    public class SoDuDto
    {
        public string MaTK { get; set; } = "";
        public decimal DuNo { get; set; }
        public decimal DuCo { get; set; }
    }
}
