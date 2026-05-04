using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeborahAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddDoanhThuBanHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMHH",
                columns: table => new
                {
                    MaHH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenHH = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Dvt = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMHH", x => x.MaHH);
                });

            migrationBuilder.CreateTable(
                name: "DMKH",
                columns: table => new
                {
                    MaKH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenKH = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaSoThue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SoTKNH = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMKH", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "HDHH",
                columns: table => new
                {
                    SoHD = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayHD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TKNoThanhToan = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKCoDoanhThu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKCoThue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKChietKhau = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TienCK = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TyLeCK = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TienThanhToan = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TienDoanhThu = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TienThue = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    ThueSuat = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    DienGiai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HTTT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HDHH", x => x.SoHD);
                    table.ForeignKey(
                        name: "FK_HDHH_DMKH_MaKH",
                        column: x => x.MaKH,
                        principalTable: "DMKH",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuGiamGia",
                columns: table => new
                {
                    SoPhieu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayPhieu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TKNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKNoThue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKCo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DienGiai = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CTLQ = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuGiamGia", x => x.SoPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuGiamGia_DMKH_MaKH",
                        column: x => x.MaKH,
                        principalTable: "DMKH",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraLai",
                columns: table => new
                {
                    SoPhieu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayPhieu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TKNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKNoThue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TKCo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DienGiai = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CTLQ = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TongTien = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraLai", x => x.SoPhieu);
                    table.ForeignKey(
                        name: "FK_TraLai_DMKH_MaKH",
                        column: x => x.MaKH,
                        principalTable: "DMKH",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTHoaDon",
                columns: table => new
                {
                    SoHD = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaHH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoLuong = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTHoaDon", x => new { x.SoHD, x.MaHH });
                    table.ForeignKey(
                        name: "FK_CTHoaDon_DMHH_MaHH",
                        column: x => x.MaHH,
                        principalTable: "DMHH",
                        principalColumn: "MaHH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTHoaDon_HDHH_SoHD",
                        column: x => x.SoHD,
                        principalTable: "HDHH",
                        principalColumn: "SoHD",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTGiamGia",
                columns: table => new
                {
                    SoPhieu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaHH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoLuong = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTGiamGia", x => new { x.SoPhieu, x.MaHH });
                    table.ForeignKey(
                        name: "FK_CTGiamGia_DMHH_MaHH",
                        column: x => x.MaHH,
                        principalTable: "DMHH",
                        principalColumn: "MaHH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTGiamGia_PhieuGiamGia_SoPhieu",
                        column: x => x.SoPhieu,
                        principalTable: "PhieuGiamGia",
                        principalColumn: "SoPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTTraLai",
                columns: table => new
                {
                    SoPhieu = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaHH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoLuong = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTTraLai", x => new { x.SoPhieu, x.MaHH });
                    table.ForeignKey(
                        name: "FK_CTTraLai_DMHH_MaHH",
                        column: x => x.MaHH,
                        principalTable: "DMHH",
                        principalColumn: "MaHH",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTTraLai_TraLai_SoPhieu",
                        column: x => x.SoPhieu,
                        principalTable: "TraLai",
                        principalColumn: "SoPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CTGiamGia_MaHH",
                table: "CTGiamGia",
                column: "MaHH");

            migrationBuilder.CreateIndex(
                name: "IX_CTHoaDon_MaHH",
                table: "CTHoaDon",
                column: "MaHH");

            migrationBuilder.CreateIndex(
                name: "IX_CTTraLai_MaHH",
                table: "CTTraLai",
                column: "MaHH");

            migrationBuilder.CreateIndex(
                name: "IX_HDHH_MaKH",
                table: "HDHH",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuGiamGia_MaKH",
                table: "PhieuGiamGia",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_TraLai_MaKH",
                table: "TraLai",
                column: "MaKH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CTGiamGia");

            migrationBuilder.DropTable(
                name: "CTHoaDon");

            migrationBuilder.DropTable(
                name: "CTTraLai");

            migrationBuilder.DropTable(
                name: "PhieuGiamGia");

            migrationBuilder.DropTable(
                name: "HDHH");

            migrationBuilder.DropTable(
                name: "DMHH");

            migrationBuilder.DropTable(
                name: "TraLai");

            migrationBuilder.DropTable(
                name: "DMKH");
        }
    }
}
