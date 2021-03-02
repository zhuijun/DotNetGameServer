using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false, comment: "角色Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false, comment: "用户Id"),
                    NickName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "昵称"),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRole", x => x.Id);
                },
                comment: "游戏角色表");

            migrationBuilder.CreateTable(
                name: "WatermelonConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "newid()"),
                    FruitId = table.Column<int>(type: "int", nullable: false, comment: "水果Id"),
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

            migrationBuilder.CreateIndex(
                name: "IX_WatermelonConfig_FruitId",
                table: "WatermelonConfig",
                column: "FruitId",
                filter: "[FruitId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRole");

            migrationBuilder.DropTable(
                name: "WatermelonConfig");
        }
    }
}
