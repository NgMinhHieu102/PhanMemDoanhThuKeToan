using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeborahAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMBP",
                columns: table => new
                {
                    MaBP = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TenBP = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMBP", x => x.MaBP);
                });

            migrationBuilder.CreateTable(
                name: "DMKho",
                columns: table => new
                {
                    MaKho = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TenKho = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMKho", x => x.MaKho);
                });

            migrationBuilder.CreateTable(
                name: "DMNCC",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TenNCC = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DienThoai = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    MaSoThue = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMNCC", x => x.MaNCC);
                });

            migrationBuilder.CreateTable(
                name: "DMTK",
                columns: table => new
                {
                    MaTK = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TenTK = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CapTK = table.Column<int>(type: "int", nullable: false),
                    TKCapTren = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMTK", x => x.MaTK);
                });

            migrationBuilder.CreateTable(
                name: "DMVT",
                columns: table => new
                {
                    MaVT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TenVT = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DVT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVT", x => x.MaVT);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    TenDN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenNguoiDung = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Quyen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.TenDN);
                });

            migrationBuilder.CreateTable(
                name: "PhieuKT",
                columns: table => new
                {
                    SoCT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    NgayCT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CTLQ = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuKT", x => x.SoCT);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSP = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TenSP = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.MaSP);
                });

            migrationBuilder.CreateTable(
                name: "BBKK",
                columns: table => new
                {
                    SoCT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    NgayCT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiKK1 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    NguoiKK2 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    NguoiKK3 = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BBKK", x => x.SoCT);
                    table.ForeignKey(
                        name: "FK_BBKK_DMKho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "DMKho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoDuDauKy",
                columns: table => new
                {
                    MaTK = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    DuNo = table.Column<decimal>(type: "numeric(12,0)", nullable: false),
                    DuCo = table.Column<decimal>(type: "numeric(12,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoDuDauKy", x => x.MaTK);
                    table.ForeignKey(
                        name: "FK_SoDuDauKy_DMTK_MaTK",
                        column: x => x.MaTK,
                        principalTable: "DMTK",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TonDauKy",
                columns: table => new
                {
                    MaVT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    SoLuong = table.Column<decimal>(type: "numeric(10,0)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(12,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TonDauKy", x => new { x.MaVT, x.MaKho });
                    table.ForeignKey(
                        name: "FK_TonDauKy_DMKho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "DMKho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TonDauKy_DMVT_MaVT",
                        column: x => x.MaVT,
                        principalTable: "DMVT",
                        principalColumn: "MaVT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTPhieu",
                columns: table => new
                {
                    SoCT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TKCo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TKNo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CTLQ = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SoTien = table.Column<decimal>(type: "numeric(12,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTPhieu", x => new { x.SoCT, x.TKNo, x.TKCo });
                    table.ForeignKey(
                        name: "FK_CTPhieu_PhieuKT_SoCT",
                        column: x => x.SoCT,
                        principalTable: "PhieuKT",
                        principalColumn: "SoCT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhapXuat",
                columns: table => new
                {
                    SoCT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    NgayCT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaKho = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MaNCC = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MaSP = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CTLQ = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhapXuat", x => x.SoCT);
                    table.ForeignKey(
                        name: "FK_PhieuNhapXuat_DMKho_MaKho",
                        column: x => x.MaKho,
                        principalTable: "DMKho",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuNhapXuat_DMNCC_MaNCC",
                        column: x => x.MaNCC,
                        principalTable: "DMNCC",
                        principalColumn: "MaNCC",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuNhapXuat_SanPham_MaSP",
                        column: x => x.MaSP,
                        principalTable: "SanPham",
                        principalColumn: "MaSP",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTBBKK",
                columns: table => new
                {
                    SoCT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MaVT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    SoLuongKiemKe = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTBBKK", x => new { x.SoCT, x.MaVT });
                    table.ForeignKey(
                        name: "FK_CTBBKK_BBKK_SoCT",
                        column: x => x.SoCT,
                        principalTable: "BBKK",
                        principalColumn: "SoCT",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTBBKK_DMVT_MaVT",
                        column: x => x.MaVT,
                        principalTable: "DMVT",
                        principalColumn: "MaVT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CTPHIEUNX",
                columns: table => new
                {
                    SoCT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    MaVT = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TKNo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    TKCo = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    SoLuong = table.Column<decimal>(type: "numeric(10,0)", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(10,0)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(12,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CTPHIEUNX", x => new { x.SoCT, x.MaVT });
                    table.ForeignKey(
                        name: "FK_CTPHIEUNX_DMVT_MaVT",
                        column: x => x.MaVT,
                        principalTable: "DMVT",
                        principalColumn: "MaVT",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CTPHIEUNX_PhieuNhapXuat_SoCT",
                        column: x => x.SoCT,
                        principalTable: "PhieuNhapXuat",
                        principalColumn: "SoCT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NguoiDung",
                columns: new[] { "TenDN", "MatKhau", "Quyen", "TenNguoiDung" },
                values: new object[] { "admin", "admin123", 1, "Quản trị viên" });

            migrationBuilder.CreateIndex(
                name: "IX_BBKK_MaKho",
                table: "BBKK",
                column: "MaKho");

            migrationBuilder.CreateIndex(
                name: "IX_CTBBKK_MaVT",
                table: "CTBBKK",
                column: "MaVT");

            migrationBuilder.CreateIndex(
                name: "IX_CTPHIEUNX_MaVT",
                table: "CTPHIEUNX",
                column: "MaVT");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapXuat_MaKho",
                table: "PhieuNhapXuat",
                column: "MaKho");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapXuat_MaNCC",
                table: "PhieuNhapXuat",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapXuat_MaSP",
                table: "PhieuNhapXuat",
                column: "MaSP");

            migrationBuilder.CreateIndex(
                name: "IX_TonDauKy_MaKho",
                table: "TonDauKy",
                column: "MaKho");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CTBBKK");

            migrationBuilder.DropTable(
                name: "CTPhieu");

            migrationBuilder.DropTable(
                name: "CTPHIEUNX");

            migrationBuilder.DropTable(
                name: "DMBP");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "SoDuDauKy");

            migrationBuilder.DropTable(
                name: "TonDauKy");

            migrationBuilder.DropTable(
                name: "BBKK");

            migrationBuilder.DropTable(
                name: "PhieuKT");

            migrationBuilder.DropTable(
                name: "PhieuNhapXuat");

            migrationBuilder.DropTable(
                name: "DMTK");

            migrationBuilder.DropTable(
                name: "DMVT");

            migrationBuilder.DropTable(
                name: "DMKho");

            migrationBuilder.DropTable(
                name: "DMNCC");

            migrationBuilder.DropTable(
                name: "SanPham");
        }
    }
}
