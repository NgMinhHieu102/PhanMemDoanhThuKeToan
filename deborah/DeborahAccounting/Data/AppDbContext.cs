using Microsoft.EntityFrameworkCore;
using DeborahAccounting.Models;

namespace DeborahAccounting.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<DMTK> DMTK { get; set; }
        public DbSet<DMNCC> DMNCC { get; set; }
        public DbSet<DMKho> DMKho { get; set; }
        public DbSet<DMVT> DMVT { get; set; }
        public DbSet<DMBP> DMBP { get; set; }
        public DbSet<SanPham> SanPham { get; set; }
        public DbSet<PhieuNhapXuat> PhieuNhapXuat { get; set; }
        public DbSet<CTPHIEUNX> CTPHIEUNX { get; set; }
        public DbSet<BBKK> BBKK { get; set; }
        public DbSet<CTBBKK> CTBBKK { get; set; }
        public DbSet<PhieuKT> PhieuKT { get; set; }
        public DbSet<CTPhieu> CTPhieu { get; set; }
        public DbSet<TonDauKy> TonDauKy { get; set; }
        public DbSet<SoDuDauKy> SoDuDauKy { get; set; }

        // Kế toán doanh thu bán hàng
        public DbSet<DMKH> DMKH { get; set; }
        public DbSet<DMHH> DMHH { get; set; }
        public DbSet<HDHH> HDHH { get; set; }
        public DbSet<CTHoaDon> CTHoaDon { get; set; }
        public DbSet<TraLai> TraLai { get; set; }
        public DbSet<CTTraLai> CTTraLai { get; set; }
        public DbSet<PhieuGiamGia> PhieuGiamGia { get; set; }
        public DbSet<CTGiamGia> CTGiamGia { get; set; }

        // Cập nhật
        public DbSet<BieuThue> BieuThue { get; set; }
        public DbSet<BangGiaBan> BangGiaBan { get; set; }
        public DbSet<CKTM> CKTM { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite keys
            modelBuilder.Entity<CTPHIEUNX>().HasKey(c => new { c.SoCT, c.MaVT });
            modelBuilder.Entity<CTBBKK>().HasKey(c => new { c.SoCT, c.MaVT });
            modelBuilder.Entity<CTPhieu>().HasKey(c => new { c.SoCT, c.TKNo, c.TKCo });
            modelBuilder.Entity<TonDauKy>().HasKey(t => new { t.MaVT, t.MaKho });

            // Composite keys - Doanh thu bán hàng
            modelBuilder.Entity<CTHoaDon>().HasKey(c => new { c.SoHD, c.MaHH });
            modelBuilder.Entity<CTTraLai>().HasKey(c => new { c.SoPhieu, c.MaHH });
            modelBuilder.Entity<CTGiamGia>().HasKey(c => new { c.SoPhieu, c.MaHH });
            modelBuilder.Entity<BangGiaBan>().HasKey(b => new { b.MaHH, b.NgayHLuc });

            // Seed tài khoản admin mặc định
            modelBuilder.Entity<NguoiDung>().HasData(new NguoiDung
            {
                TenDN = "admin",
                MatKhau = "admin123",
                TenNguoiDung = "Quản trị viên",
                Quyen = 1 // 1: Admin
            });
        }
    }
}
