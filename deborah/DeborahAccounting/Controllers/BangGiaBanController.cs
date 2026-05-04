using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Data;
using DeborahAccounting.Models;

namespace DeborahAccounting.Controllers
{
    [Authorize]
    public class BangGiaBanController : Controller
    {
        private readonly AppDbContext _db;
        public BangGiaBanController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.BangGiaBan.Include(b => b.HangHoa).OrderBy(b => b.MaHH).ThenBy(b => b.NgayHLuc).ToListAsync();
            ViewBag.DSHH = await _db.DMHH.OrderBy(x => x.MaHH).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string maHH, DateTime ngayHLuc, decimal giaBan)
        {
            if (await _db.BangGiaBan.AnyAsync(x => x.MaHH == maHH && x.NgayHLuc == ngayHLuc))
                return Json(new { success = false, message = "Giá bán cho mã HH và ngày này đã tồn tại" });
            _db.BangGiaBan.Add(new BangGiaBan { MaHH = maHH, NgayHLuc = ngayHLuc, GiaBan = giaBan });
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string maHH, DateTime ngayHLuc)
        {
            var item = await _db.BangGiaBan.FindAsync(maHH, ngayHLuc);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });
            _db.BangGiaBan.Remove(item);
            await _db.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
