using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class WatermelonConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WatermelonConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rate = table.Column<int>(type: "int", nullable: false, comment: "概率"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "名称"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "图片"),
                    Score = table.Column<int>(type: "int", nullable: false, comment: "合成可得的积分")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatermelonConfig", x => x.Id);
                },
                comment: "合成大西瓜配置表");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatermelonConfig");
        }
    }
}
