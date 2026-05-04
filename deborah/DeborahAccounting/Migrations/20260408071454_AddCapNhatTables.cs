using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeborahAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddCapNhatTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangGiaBan",
                columns: table => new
                {
                    MaHH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayHLuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaBan = table.Column<decimal>(type: "numeric(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGiaBan", x => new { x.MaHH, x.NgayHLuc });
                    table.ForeignKey(
                        name: "FK_BangGiaBan_DMHH_MaHH",
                        column: x => x.MaHH,
                        principalTable: "DMHH",
                        principalColumn: "MaHH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BieuThue",
                columns: table => new
                {
                    MaThue = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenThue = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ThueSuat = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BieuThue", x => x.MaThue);
                });

            migrationBuilder.CreateTable(
                name: "CKTM",
                columns: table => new
                {
                    SoBang = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaKH = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NgayHLuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TyleCK = table.Column<decimal>(type: "numeric(10,0)", nullable: false),
                    TienCK = table.Column<decimal>(type: "numeric(10,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CKTM", x => x.SoBang);
                    table.ForeignKey(
                        name: "FK_CKTM_DMKH_MaKH",
                        column: x => x.MaKH,
                        principalTable: "DMKH",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CKTM_MaKH",
                table: "CKTM",
                column: "MaKH");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangGiaBan");

            migrationBuilder.DropTable(
                name: "BieuThue");

            migrationBuilder.DropTable(
                name: "CKTM");
        }
    }
}
